using System.Text.RegularExpressions;

namespace Cohesion.Good;

// ✅ PART 1 - Functional cohesion: things that change together, live together.
//    Behaviour moves next to the data/concept it belongs to.

public record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new ArgumentException($"'{value}' is not a valid e-mail address");
        Value = value;
    }
}

public record Price(decimal ExcludingVat)
{
    private const decimal VatRate = 0.25m;
    public decimal IncludingVat => ExcludingVat * (1 + VatRate);
}

public record CardNumber(string Value)
{
    public string Masked => new string('*', Value.Length - 4) + Value[^4..];
}

// ✅ PART 2 - Split the god object along its field clusters.
//    Every method in each class uses most of that class' fields => high cohesion.

public class Credentials(string username, string passwordHash)
{
    private const int MaxFailedLogins = 3;
    private string _passwordHash = passwordHash;
    private int _failedLogins;

    public string Username { get; } = username;

    public bool Login(string password)
    {
        if (_failedLogins >= MaxFailedLogins) return false;
        var ok = _passwordHash == Hash(password);
        _failedLogins = ok ? 0 : _failedLogins + 1;
        return ok;
    }

    public void ChangePassword(string newPassword) => _passwordHash = Hash(newPassword);

    public static string Hash(string s) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s));
}

public class BillingAccount(CardNumber card)
{
    private readonly List<decimal> _invoices = [];
    private decimal _balance;

    public void Charge(decimal amount)
    {
        _balance -= amount;
        _invoices.Add(amount);
    }

    public string Summary() => $"Card {card.Masked}, {_invoices.Count} invoices, balance {_balance}";
}

public class Preferences
{
    public string Language { get; set; } = "da";
    public bool DarkMode { get; private set; }

    public void ToggleDarkMode() => DarkMode = !DarkMode;
    public string Greeting() => Language == "da" ? "Hej" : "Hello";
}

// The aggregate just composes the cohesive parts.
public class UserAccount(Credentials credentials, BillingAccount billing, Preferences preferences)
{
    public Credentials Credentials { get; } = credentials;
    public BillingAccount Billing { get; } = billing;
    public Preferences Preferences { get; } = preferences;
}
