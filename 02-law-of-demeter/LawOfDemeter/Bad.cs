namespace LawOfDemeter.Bad;

// The classic "Paperboy" example (David Bock, "The Paperboy, The Wallet, and The Law of Demeter").

public class Wallet
{
    public decimal TotalMoney { get; set; }
}

public class Customer
{
    public string Name { get; init; } = "";
    public Wallet Wallet { get; init; } = new();
    public Address Address { get; init; } = new();
}

public class Address
{
    public Street Street { get; init; } = new();
}

public class Street
{
    public string Name { get; init; } = "";
    public int Number { get; init; }
}

public class Paperboy
{
    public decimal Collected { get; private set; }

    public void CollectPayment(Customer customer, decimal amount)
    {
        // ❌ The paperboy grabs the customer's wallet, looks inside and takes the money.
        //    - Paperboy now knows that customers HAVE wallets (what about MobilePay? credit?)
        //    - Paperboy knows wallets hold a decimal called TotalMoney
        //    - Paperboy enforces the "not enough money" rule - for the customer
        //    - A wallet could be null tomorrow, and every paperboy-like class must handle it
        if (customer.Wallet.TotalMoney >= amount)
        {
            customer.Wallet.TotalMoney -= amount;
            Collected += amount;
        }
        else
        {
            Console.WriteLine($"{customer.Name} can't pay, come back tomorrow");
        }
    }

    public string RouteLabel(Customer customer) =>
        // ❌ "Train wreck": navigates three objects deep. A change to how
        //    addresses are modelled ripples all the way out here.
        $"{customer.Address.Street.Name} {customer.Address.Street.Number}";
}
