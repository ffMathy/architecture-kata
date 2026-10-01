namespace LawOfDemeter.Good;

// ✅ "Only talk to your immediate friends."
//    A method M of object O may only call methods on:
//      - O itself
//      - M's parameters
//      - objects M creates
//      - O's own fields

public class Wallet
{
    private decimal _money;

    public Wallet(decimal money) => _money = money;

    internal bool TryTake(decimal amount)
    {
        if (_money < amount) return false;
        _money -= amount;
        return true;
    }
}

public record Address(string Street, int Number)
{
    public string ToRouteLabel() => $"{Street} {Number}";
}

public class Customer(string name, Wallet wallet, Address address)
{
    public string Name { get; } = name;

    // The customer decides HOW to pay. Wallet, MobilePay, or tab - not the paperboy's business.
    public bool TryPay(decimal amount) => wallet.TryTake(amount);

    public string DeliveryLabel() => address.ToRouteLabel();
}

public class Paperboy
{
    public decimal Collected { get; private set; }

    public void CollectPayment(Customer customer, decimal amount)
    {
        if (customer.TryPay(amount))
            Collected += amount;
        else
            Console.WriteLine($"{customer.Name} can't pay, come back tomorrow");
    }

    public string RouteLabel(Customer customer) => customer.DeliveryLabel();
}
