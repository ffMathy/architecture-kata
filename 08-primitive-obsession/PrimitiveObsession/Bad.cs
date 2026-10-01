namespace PrimitiveObsession.Bad;

// ❌ Everything is a string, int or decimal. The compiler can't tell them apart.
public class OrderService
{
    // Quick: which argument is which? Did you get the order right at every call site?
    public void PlaceOrder(int customerId, int productId, int quantity, decimal amount, string currency, string email)
    {
        // Validation is copied into every method that receives an email or an amount...
        if (!email.Contains('@')) throw new ArgumentException("Invalid email");
        if (amount < 0) throw new ArgumentException("Negative amount");
        if (currency.Length != 3) throw new ArgumentException("Invalid currency");

        Console.WriteLine($"Order: customer={customerId} product={productId} qty={quantity} {amount} {currency} -> {email}");
    }

    public decimal Total(decimal priceA, string currencyA, decimal priceB, string currencyB) =>
        // ...and nothing stops you adding DKK to EUR.
        priceA + priceB;
}
