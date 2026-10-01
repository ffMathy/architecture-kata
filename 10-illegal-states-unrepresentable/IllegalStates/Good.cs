namespace IllegalStates.Good;

// ✅ Each state is its own type and carries exactly the data that state requires.
//    Transitions are methods that only exist on the states they're valid from.
//    (A poor man's discriminated union - C# doesn't have native unions yet.)
public abstract record Order(Guid Id)
{
    public sealed record Pending(Guid Id) : Order(Id)
    {
        public Paid Pay(string paymentReference, DateTime at) => new(Id, paymentReference, at);
        public Cancelled Cancel(string reason) => new(Id, reason);
    }

    public sealed record Paid(Guid Id, string PaymentReference, DateTime PaidAt) : Order(Id)
    {
        public Shipped Ship(string trackingNumber, DateTime at) => new(Id, PaymentReference, trackingNumber, at);
        public Cancelled Cancel(string reason) => new(Id, $"{reason} (refund {PaymentReference})");
    }

    public sealed record Shipped(Guid Id, string PaymentReference, string TrackingNumber, DateTime ShippedAt) : Order(Id);
    // No Cancel() on Shipped: you cannot cancel a shipped order. It's not a runtime check - the method doesn't exist.

    public sealed record Cancelled(Guid Id, string Reason) : Order(Id);
}

public static class OrderPrinter
{
    // Pattern matching over a closed set. No nulls, no `!`, no typos in status strings.
    public static string Describe(Order order) => order switch
    {
        Order.Pending => "Waiting for payment",
        Order.Paid p => $"Paid ({p.PaymentReference})",
        Order.Shipped s => $"Shipped with tracking {s.TrackingNumber.ToUpper()}",
        Order.Cancelled c => $"Cancelled: {c.Reason}",
        _ => throw new System.Diagnostics.UnreachableException(),
    };
}
