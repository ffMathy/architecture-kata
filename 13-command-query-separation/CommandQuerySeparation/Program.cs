using Bad = CommandQuerySeparation.Bad;
using Good = CommandQuerySeparation.Good;

Console.WriteLine("=== BAD: queries with side effects ===");
var badNumbers = new Bad.InvoiceNumberGenerator();
Console.WriteLine($"[debug log] next number will be {badNumbers.GetNextInvoiceNumber()}"); // innocent logging...
Console.WriteLine($"Invoice created: {badNumbers.GetNextInvoiceNumber()}  <- INV-1000 was skipped 💥");

var badCart = new Bad.ShoppingCart();
badCart.Add(200); badCart.Add(100);
Console.WriteLine($"Cart page shows:     {badCart.GetTotal()}");
Console.WriteLine($"Checkout charges:    {badCart.GetTotal()}  <- displaying the total ate the discount 💥");

var badValidator = new Bad.UserValidator();
foreach (var source in new[] { "form validation", "audit log", "login" })
    Console.WriteLine($"IsValid(\"wrong\") called by {source,-16} -> FailedAttempts = {(badValidator.IsValid("wrong") ? 0 : badValidator.FailedAttempts)}");
Console.WriteLine($"User types the right password:  IsValid = {badValidator.IsValid("hunter2")}  <- locked out by our own checks 💥");

Console.WriteLine();
Console.WriteLine("=== GOOD: queries are safe, commands are explicit ===");
var goodNumbers = new Good.InvoiceNumberGenerator();
Console.WriteLine($"[debug log] next number will be {goodNumbers.PeekNextInvoiceNumber()}");
Console.WriteLine($"Invoice created: {goodNumbers.ReserveInvoiceNumber()}  ✅");

var goodCart = new Good.ShoppingCart();
goodCart.Add(200); goodCart.Add(100);
goodCart.ApplyWelcomeDiscount();
Console.WriteLine($"Cart page shows:     {goodCart.Total}");
Console.WriteLine($"Checkout charges:    {goodCart.Total}  ✅");

var goodValidator = new Good.UserValidator();
Console.WriteLine($"Matches(\"wrong\") x3 for validation/audit -> FailedAttempts = {(goodValidator.Matches("wrong") | goodValidator.Matches("wrong") | goodValidator.Matches("wrong") ? -1 : goodValidator.FailedAttempts)}");
Console.WriteLine($"TryLogin(\"hunter2\") = {goodValidator.TryLogin("hunter2")}  ✅");
