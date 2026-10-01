using Bad = Encapsulation.Bad;
using Good = Encapsulation.Good;

Console.WriteLine("=== BAD: public setters, rules scattered across services ===");
var bad = new Bad.BankAccount { Owner = "Jane", Balance = 1000 };
new Bad.AtmService().Withdraw(bad, 100);
bad.IsFrozen = true;                         // fraud team freezes the account...
new Bad.MobilePayService().Pay(bad, 300);    // ...MobilePay ignores it
Console.WriteLine($"After MobilePay on a FROZEN account: balance {bad.Balance}, transactions={bad.Transactions.Count} - paid anyway, and not recorded 💥");
bad.Transactions.Clear();                    // anybody can rewrite history
bad.Balance = 1_000_000;                     // or just set a new balance
Console.WriteLine($"Balance {bad.Balance}, frozen={bad.IsFrozen}, transactions={bad.Transactions.Count}  💥 invariants broken");

Console.WriteLine();
Console.WriteLine("=== GOOD: state only changes through behaviour ===");
var good = new Good.BankAccount("Jane");
good.Deposit(1000, "Salary");
new Good.AtmService().Withdraw(good, 100);
good.Freeze();
try
{
    new Good.MobilePayService().Pay(good, 300);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"MobilePay rejected: {ex.Message}  ✅");
}
// good.Balance = 1_000_000;            <- does not compile
// good.Transactions.Add(...);          <- does not compile (IReadOnlyList)
Console.WriteLine($"Balance {good.Balance}, frozen={good.IsFrozen}, transactions={good.Transactions.Count}");
