using Bad = DependencyInversion.Bad;
using Good = DependencyInversion.Good;

Console.WriteLine("=== BAD: business logic news up its infrastructure ===");
new Bad.OrderService().PlaceOrder(new(1, "jane@example.com", 199m));

Console.WriteLine();
Console.WriteLine("=== GOOD: composition root decides the details ===");

// Production wiring
new Good.OrderService(new Good.SqlServerOrderRepository(), new Good.SmtpNotifier())
    .PlaceOrder(new(2, "jane@example.com", 199m));

// Business wants SMS instead - OrderService is untouched
new Good.OrderService(new Good.SqlServerOrderRepository(), new Good.SmsNotifier())
    .PlaceOrder(new(3, "jane@example.com", 199m));

// "Unit test" - no database, no mail server
var repo = new Good.InMemoryOrderRepository();
var spy = new Good.SpyNotifier();
new Good.OrderService(repo, spy).PlaceOrder(new(4, "test@example.com", 10m));
Console.WriteLine($"[Test] stored={repo.Orders.Count} notified=[{string.Join(",", spy.Notified)}]  ✅");
