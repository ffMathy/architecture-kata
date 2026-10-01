namespace DependencyInversion.Bad;

public record Order(int Id, string CustomerEmail, decimal Total);

// Low-level details...
public class SqlServerOrderRepository
{
    public SqlServerOrderRepository() =>
        Console.WriteLine("[SQL] Opening connection to Server=prod-db;Database=Shop (hope you're online!)");

    public void Insert(Order order) => Console.WriteLine($"[SQL] INSERT INTO Orders VALUES ({order.Id}, {order.Total})");
}

public class SmtpMailer
{
    public void SendMail(string to, string subject) => Console.WriteLine($"[SMTP] To: {to} Subject: {subject}");
}

// ❌ The high-level policy ("placing an order") depends directly on
//    low-level details. You can't:
//      - unit test it without a SQL Server and an SMTP server
//      - switch to Postgres or SMS without editing business logic
//      - reuse it in another context (e.g. a batch import)
public class OrderService
{
    private readonly SqlServerOrderRepository _repository = new();
    private readonly SmtpMailer _mailer = new();

    public void PlaceOrder(Order order)
    {
        if (order.Total <= 0) throw new ArgumentException("Order total must be positive");

        _repository.Insert(order);
        _mailer.SendMail(order.CustomerEmail, $"Order {order.Id} confirmed");
    }
}
