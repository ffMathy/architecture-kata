namespace PrimitiveObsession.Good;

// ✅ Small value objects: validated once, impossible to mix up, and a home for behaviour.

public readonly record struct CustomerId(int Value);
public readonly record struct ProductId(int Value);

public readonly record struct Quantity
{
    public int Value { get; }
    public Quantity(int value) =>
        Value = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "Quantity must be positive");
}

public sealed record Email
{
    public string Value { get; }
    public Email(string value) =>
        Value = value.Contains('@') ? value.Trim().ToLowerInvariant() : throw new ArgumentException($"'{value}' is not an email");
    public override string ToString() => Value;
}

public enum Currency { DKK, EUR, USD }

public readonly record struct Money(decimal Amount, Currency Currency)
{
    public static Money operator +(Money a, Money b) =>
        a.Currency == b.Currency
            ? new Money(a.Amount + b.Amount, a.Currency)
            : throw new InvalidOperationException($"Cannot add {a.Currency} to {b.Currency}");

    public static Money operator *(Money m, Quantity q) => m with { Amount = m.Amount * q.Value };

    public override string ToString() => $"{Amount:0.00} {Currency}";
}

public class OrderService
{
    // The signature documents itself, and the compiler checks every call site.
    public void PlaceOrder(CustomerId customer, ProductId product, Quantity quantity, Money unitPrice, Email email)
    {
        // No validation needed here - invalid values can't exist.
        Console.WriteLine($"Order: customer={customer.Value} product={product.Value} qty={quantity.Value} {unitPrice * quantity} -> {email}");
    }
}
