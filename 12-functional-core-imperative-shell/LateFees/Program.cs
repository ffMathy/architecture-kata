using Bad = LateFees.Bad;
using Good = LateFees.Good;

Console.WriteLine("=== BAD: logic tangled with I/O and the clock ===");
new Bad.LateFeeJob().Run();

Console.WriteLine();
Console.WriteLine("=== GOOD: pure core, thin shell ===");
var today = DateOnly.FromDateTime(DateTime.Today);
var store = new Good.InMemoryLoanStore(
[
    new(1, "jane@example.com", today.AddDays(-3), FeeCharged: false),
    new(2, "bob@example.com", today.AddDays(-20), FeeCharged: false),
    new(3, "eve@example.com", today.AddDays(5), FeeCharged: false),
]);
new Good.LateFeeJob(store, new Good.ConsoleMailer(), new Good.ConsolePayments(), TimeProvider.System).Run();

Console.WriteLine();
Console.WriteLine("The core can also be called directly - e.g. for a 'preview' screen, with no side effects:");
foreach (var decision in Good.LateFeePolicy.Decide(store.GetOpenLoans(), today.AddDays(30)))
    Console.WriteLine($"  {decision}");

Console.WriteLine();
Console.WriteLine("Run the tests: dotnet test 12-functional-core-imperative-shell/LateFees.Tests");
