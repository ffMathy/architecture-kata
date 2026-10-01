using Bad = PrimitiveObsession.Bad;
using Good = PrimitiveObsession.Good;

Console.WriteLine("=== BAD: primitives everywhere ===");
var badService = new Bad.OrderService();
int customerId = 42, productId = 7;
badService.PlaceOrder(customerId, productId, 2, 100m, "DKK", "jane@example.com");
badService.PlaceOrder(productId, customerId, 2, 100m, "DKK", "jane@example.com"); // swapped - compiles, ships, 💥 in prod
Console.WriteLine($"100 DKK + 100 EUR = {badService.Total(100m, "DKK", 100m, "EUR")} (of... what?) 💥");

Console.WriteLine();
Console.WriteLine("=== GOOD: value objects ===");
var goodService = new Good.OrderService();
var customer = new Good.CustomerId(42);
var product = new Good.ProductId(7);
goodService.PlaceOrder(customer, product, new(2), new(100m, Good.Currency.DKK), new("Jane@Example.com"));
// goodService.PlaceOrder(product, customer, ...);   <- compile error: cannot convert ProductId to CustomerId

try
{
    _ = new Good.Money(100, Good.Currency.DKK) + new Good.Money(100, Good.Currency.EUR);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"{ex.Message} ✅");
}

try
{
    _ = new Good.Email("not-an-email");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"{ex.Message} ✅ (rejected at the edge, once)");
}
