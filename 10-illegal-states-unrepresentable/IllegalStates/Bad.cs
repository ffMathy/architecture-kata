namespace IllegalStates.Bad;

// ❌ One class with a status string and a pile of nullable fields.
//    Count the combinations: 4 statuses x 2^5 null/non-null fields = 128 possible states.
//    The business has exactly 4 valid ones.
public class Order
{
    public string Status { get; set; } = "Pending";   // "Pending" | "Paid" | "Shipped" | "Cancelled" | "Shiped" ...
    public DateTime? PaidAt { get; set; }
    public string? PaymentReference { get; set; }
    public DateTime? ShippedAt { get; set; }
    public string? TrackingNumber { get; set; }
    public string? CancellationReason { get; set; }
}

public static class OrderPrinter
{
    public static string Describe(Order o) => o.Status switch
    {
        "Pending" => "Waiting for payment",
        "Paid" => $"Paid ({o.PaymentReference})",
        // Is TrackingNumber set? It should be... probably. `!` and pray.
        "Shipped" => $"Shipped with tracking {o.TrackingNumber!.ToUpper()}",
        "Cancelled" => $"Cancelled: {o.CancellationReason}",
        _ => "¯\\_(ツ)_/¯",
    };
}
