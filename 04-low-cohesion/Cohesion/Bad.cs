using System.Text.RegularExpressions;

namespace Cohesion.Bad;

// ❌ PART 1 - Coincidental cohesion: the "Utils" / "Helpers" / "Manager" junk drawer.
//    The only thing these methods have in common is that someone needed a place to put them.
public static class Helpers
{
    public static bool IsValidEmail(string s) => Regex.IsMatch(s, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    public static decimal AddVat(decimal amount) => amount * 1.25m;
    public static string FormatDanishDate(DateTime d) => d.ToString("dd-MM-yyyy");
    public static string Slugify(string title) => Regex.Replace(title.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
    public static int CalculateAge(DateTime birthday) => (int)((DateTime.Today - birthday).TotalDays / 365.25);
    public static string MaskCreditCard(string number) => new string('*', number.Length - 4) + number[^4..];
}

// ❌ PART 2 - Low cohesion at class level: a "god object" whose fields fall
//    into separate clusters. Each method only touches ONE cluster.
//    (LCOM - Lack of Cohesion of Methods - would be high.)
public class UserAccount
{
    // cluster A: identity / authentication
    public string Username = "";
    public string PasswordHash = "";
    public int FailedLogins;

    // cluster B: billing
    public string CardNumber = "";
    public decimal Balance;
    public List<decimal> Invoices = [];

    // cluster C: preferences
    public string Language = "da";
    public bool DarkMode;
    public bool NewsletterOptIn;

    public bool Login(string password)
    {                                                   // touches: A
        if (FailedLogins >= 3) return false;
        var ok = PasswordHash == Hash(password);
        FailedLogins = ok ? 0 : FailedLogins + 1;
        return ok;
    }

    public void ChangePassword(string newPassword) =>   // touches: A
        PasswordHash = Hash(newPassword);

    public void Charge(decimal amount)
    {                                                   // touches: B
        Balance -= amount;
        Invoices.Add(amount);
    }

    public string BillingSummary() =>                   // touches: B
        $"Card {Helpers.MaskCreditCard(CardNumber)}, {Invoices.Count} invoices, balance {Balance}";

    public void ToggleDarkMode() => DarkMode = !DarkMode;            // touches: C
    public string Greeting() => Language == "da" ? "Hej" : "Hello";  // touches: C

    private static string Hash(string s) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s));
}
