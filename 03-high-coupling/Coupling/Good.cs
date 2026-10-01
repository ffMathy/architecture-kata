namespace Coupling.Good;

// ✅ LOW COUPLING:
//    - collaborators are injected abstractions (no `new`, no statics)
//    - only the data needed is passed (data coupling)
//    - side effects subscribe to an event instead of being hard-wired into checkout

public record OrderPlaced(string CustomerEmail, string Product, int Quantity, decimal Total);

public interface IInventory
{
    bool TryReserve(string product, int quantity);
}

public interface IPayments
{
    void Charge(string customerEmail, decimal amount);
}

/// <summary>A tiny in-process event bus. In real life: MediatR notifications, MassTransit, a message broker...</summary>
public class EventBus
{
    private readonly List<Action<OrderPlaced>> _handlers = [];
    public void Subscribe(Action<OrderPlaced> handler) => _handlers.Add(handler);
    public void Publish(OrderPlaced e) => _handlers.ForEach(h => h(e));
}

public class CheckoutService(IInventory inventory, IPayments payments, EventBus events)
{
    // Checkout knows about exactly three things, and none of them is e-mail, loyalty or SMS.
    public void Checkout(string customerEmail, string product, int quantity, decimal price)
    {
        if (!inventory.TryReserve(product, quantity))
            throw new InvalidOperationException("Out of stock");

        var total = quantity * price;
        payments.Charge(customerEmail, total);

        events.Publish(new OrderPlaced(customerEmail, product, quantity, total));
    }
}

// ---- Implementations ------------------------------------------------------

public class InMemoryInventory : IInventory
{
    private readonly Dictionary<string, int> _stock = new() { ["coffee"] = 10, ["grinder"] = 1 };

    public bool TryReserve(string product, int quantity)
    {
        if (_stock.GetValueOrDefault(product) < quantity) return false;
        _stock[product] -= quantity;
        return true;
    }
}

public class FakePayments : IPayments
{
    public void Charge(string customerEmail, decimal amount) =>
        Console.WriteLine($"[Payments] charged {amount} to {customerEmail}");
}

// Explicit, intention-revealing methods instead of boolean control flags.
public class EmailService
{
    public void SendReceipt(OrderPlaced e) => Console.WriteLine($"[Email] {e.CustomerEmail}: Thanks for buying {e.Product}");
}

public class LoyaltyProgram
{
    // Gets everything it needs as data. No hidden ordering dependency.
    public void AwardPoints(OrderPlaced e) => Console.WriteLine($"[Loyalty] {e.CustomerEmail} earns {(int)(e.Total / 10)} points");
}

public class SmsService
{
    public void NotifyWarehouse(OrderPlaced e) => Console.WriteLine($"[SMS] warehouse: pick {e.Quantity} x {e.Product}");
}
