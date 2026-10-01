using Bad = Cohesion.Bad;
using Good = Cohesion.Good;

Console.WriteLine("=== BAD: junk-drawer helpers + god object ===");
Console.WriteLine($"Helpers.IsValidEmail: {Bad.Helpers.IsValidEmail("jane@example.com")}");
Console.WriteLine($"Helpers.AddVat:       {Bad.Helpers.AddVat(100)}");

var bad = new Bad.UserAccount { Username = "jane", PasswordHash = "c2VjcmV0", CardNumber = "4111111111111111" };
bad.Login("secret");
bad.Charge(99);
bad.ToggleDarkMode();
Console.WriteLine($"{bad.Greeting()} {bad.Username}! {bad.BillingSummary()}");
Console.WriteLine("Which team owns UserAccount.cs? Auth, Billing AND Frontend. 😬");

Console.WriteLine();
Console.WriteLine("=== GOOD: each type is about one thing ===");
var email = new Good.EmailAddress("jane@example.com");
Console.WriteLine($"EmailAddress:         {email.Value} (validated on construction)");
Console.WriteLine($"Price.IncludingVat:   {new Good.Price(100).IncludingVat}");

var user = new Good.UserAccount(
    new Good.Credentials("jane", Good.Credentials.Hash("secret")),
    new Good.BillingAccount(new Good.CardNumber("4111111111111111")),
    new Good.Preferences());

user.Credentials.Login("secret");
user.Billing.Charge(99);
user.Preferences.ToggleDarkMode();
Console.WriteLine($"{user.Preferences.Greeting()} {user.Credentials.Username}! {user.Billing.Summary()}");
