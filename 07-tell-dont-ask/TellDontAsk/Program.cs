using Bad = TellDontAsk.Bad;
using Good = TellDontAsk.Good;

var today = new DateOnly(2026, 10, 1);

Console.WriteLine("=== BAD: ask for data, decide outside, poke state back in ===");
var bad = new Bad.Subscription
{
    Plan = Bad.Plan.Free, ExpiresOn = today, AutoRenew = true, HasPaymentMethod = true, IsCancelled = true,
};
new Bad.RenewalJob().Run(bad, today);           // correctly does nothing
new Bad.AdminController().ForceRenew(bad, today); // duplicated rule, missing two conditions

Console.WriteLine();
Console.WriteLine("=== GOOD: tell the object what you want ===");
var good = new Good.Subscription(Good.Plan.Free, today, autoRenew: true, hasPaymentMethod: true);
good.Cancel();
new Good.RenewalJob().Run(good, today);
new Good.AdminController().ForceRenew(good, today);

var pro = new Good.Subscription(Good.Plan.Pro, today, autoRenew: true, hasPaymentMethod: true);
new Good.RenewalJob().Run(pro, today);
