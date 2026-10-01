namespace DependencyInversion.Good;

public record Order(int Id, string CustomerEmail, decimal Total);

// ✅ The high-level module OWNS the abstractions it needs.
//    Details depend on the policy - not the other way around.
public interface IOrderRepository
{
    void Add(Order order);
}

public interface ICustomerNotifier
{
    void OrderConfirmed(Order order);
}

public class OrderService(IOrderRepository orders, ICustomerNotifier notifier)
{
    public void PlaceOrder(Order order)
    {
        if (order.Total <= 0) throw new ArgumentException("Order total must be positive");

        orders.Add(order);
        notifier.OrderConfirmed(order);
    }
}

// ---- Details (would live in an Infrastructure project) -------------------

public class SqlServerOrderRepository : IOrderRepository
{
    public void Add(Order order) => Console.WriteLine($"[SQL] INSERT INTO Orders VALUES ({order.Id}, {order.Total})");
}

public class SmtpNotifier : ICustomerNotifier
{
    public void OrderConfirmed(Order order) =>
        Console.WriteLine($"[SMTP] To: {order.CustomerEmail} Subject: Order {order.Id} confirmed");
}

public class SmsNotifier : ICustomerNotifier
{
    public void OrderConfirmed(Order order) => Console.WriteLine($"[SMS] Your order {order.Id} is confirmed");
}

// ---- Test doubles: trivial to write because the abstractions are small ---

public class InMemoryOrderRepository : IOrderRepository
{
    public List<Order> Orders { get; } = [];
    public void Add(Order order) => Orders.Add(order);
}

public class SpyNotifier : ICustomerNotifier
{
    public List<int> Notified { get; } = [];
    public void OrderConfirmed(Order order) => Notified.Add(order.Id);
}
