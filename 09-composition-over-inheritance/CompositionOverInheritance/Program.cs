using Bad = CompositionOverInheritance.Bad;
using Good = CompositionOverInheritance.Good;

Console.WriteLine("=== BAD: one subclass per combination ===");
new Bad.RetryingLoggingSmsNotifier().Send("Your order shipped");
Console.WriteLine("  (want email AND sms with logging? write another class...)");

var badTags = new Bad.CountingTagCollection();
badTags.AddRange(["dotnet", "architecture", "kata"]);
Console.WriteLine($"Fragile base class: added 3 tags, AddAttempts = {badTags.AddAttempts} 💥");

Console.WriteLine();
Console.WriteLine("=== GOOD: compose small behaviours ===");
Good.INotifier notifier = new Good.CompositeNotifier(
    new Good.LoggingNotifier(new Good.EmailNotifier(), "email"),
    new Good.RetryingNotifier(new Good.LoggingNotifier(new Good.SmsNotifier(), "sms")));
notifier.Send("Your order shipped");

var goodTags = new Good.CountingTagCollection();
goodTags.AddRange(["dotnet", "architecture", "kata"]);
Console.WriteLine($"Wrapped collection: added 3 tags, AddAttempts = {goodTags.AddAttempts} ✅");
