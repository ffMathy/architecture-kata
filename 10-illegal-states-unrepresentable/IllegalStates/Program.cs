using Bad = IllegalStates.Bad;
using Good = IllegalStates.Good;

Console.WriteLine("=== BAD: status string + nullable fields ===");
var shippedWithoutTracking = new Bad.Order { Status = "Shipped", ShippedAt = DateTime.Now }; // perfectly "valid" object
var typo = new Bad.Order { Status = "Shiped" };
var paidAndCancelled = new Bad.Order { Status = "Cancelled", PaidAt = DateTime.Now, CancellationReason = null };

Console.WriteLine(Bad.OrderPrinter.Describe(typo));
Console.WriteLine(Bad.OrderPrinter.Describe(paidAndCancelled) + "  <- no reason, was it refunded? who knows");
try
{
    Console.WriteLine(Bad.OrderPrinter.Describe(shippedWithoutTracking));
}
catch (NullReferenceException)
{
    Console.WriteLine("💥 NullReferenceException: shipped order without a tracking number");
}

Console.WriteLine();
Console.WriteLine("=== GOOD: one type per state ===");
var pending = new Good.Order.Pending(Guid.NewGuid());
var paid = pending.Pay("PAY-123", DateTime.Now);
var shipped = paid.Ship("gls-9981", DateTime.Now);

foreach (Good.Order o in new Good.Order[] { pending, paid, shipped, paid.Cancel("Customer changed mind") })
    Console.WriteLine(Good.OrderPrinter.Describe(o));

// shipped.Cancel("too late");              <- compile error: Shipped has no Cancel
// new Good.Order.Shipped(id, "PAY-1", null!, ...)  <- you'd have to *try* to break it
