Console.WriteLine("=== BAD: switch statement that must be edited for every carrier ===");
var bad = new OpenClosed.Bad.ShippingCostCalculator();
foreach (var carrier in new[] { "PostNord", "GLS", "DHL Express", "Pickup in store" })
{
    try
    {
        Console.WriteLine($"{carrier,-16} {bad.Calculate(new(7.5m, carrier)),8:0.00} kr");
    }
    catch (NotSupportedException ex)
    {
        Console.WriteLine($"{carrier,-16} 💥 {ex.Message} (go edit the switch...)");
    }
}

Console.WriteLine();
Console.WriteLine("=== GOOD: plug in a new IShippingMethod ===");
var good = new OpenClosed.Good.ShippingCostCalculator(
[
    new OpenClosed.Good.PostNord(),
    new OpenClosed.Good.Gls(),
    new OpenClosed.Good.DhlExpress(),
    new OpenClosed.Good.PickupInStore(), // <- the only line that changed
]);
foreach (var carrier in good.AvailableCarriers)
    Console.WriteLine($"{carrier,-16} {good.Calculate(new(7.5m), carrier),8:0.00} kr");
