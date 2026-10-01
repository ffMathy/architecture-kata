using Bad = LawOfDemeter.Bad;
using Good = LawOfDemeter.Good;

Console.WriteLine("=== BAD: the paperboy reaches into the customer's wallet ===");
var alice = new Bad.Customer
{
    Name = "Alice",
    Wallet = new Bad.Wallet { TotalMoney = 100 },
    Address = new Bad.Address { Street = new Bad.Street { Name = "Elm Street", Number = 13 } },
};
var badBoy = new Bad.Paperboy();
badBoy.CollectPayment(alice, 30);
Console.WriteLine($"Delivering to {badBoy.RouteLabel(alice)}; collected {badBoy.Collected}");

// Nothing prevents anyone else from doing this either:
alice.Wallet.TotalMoney = -1_000_000;
Console.WriteLine($"Alice's wallet after a 'helpful' colleague: {alice.Wallet.TotalMoney} 💥");

Console.WriteLine();
Console.WriteLine("=== GOOD: the paperboy asks the customer to pay ===");
var bob = new Good.Customer("Bob", new Good.Wallet(100), new Good.Address("Elm Street", 13));
var goodBoy = new Good.Paperboy();
goodBoy.CollectPayment(bob, 30);
goodBoy.CollectPayment(bob, 300);
Console.WriteLine($"Delivering to {goodBoy.RouteLabel(bob)}; collected {goodBoy.Collected}");

Console.WriteLine();
Console.WriteLine("=== NOT a violation: fluent APIs / LINQ ===");
// Many dots, but every call returns the SAME kind of thing (a query) - no reaching
// into other objects' internals. LoD is not "the one-dot rule".
var names = new[] { "Carol", "alice", "Bob" }.Select(n => n.Trim()).Where(n => n.Length > 0).OrderBy(n => n.ToLower());
Console.WriteLine(string.Join(", ", names));
