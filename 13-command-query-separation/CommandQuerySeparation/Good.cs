namespace CommandQuerySeparation.Good;

// ✅ Every method is EITHER
//      a Query  - returns a value, changes nothing (safe to call any number of times), OR
//      a Command - changes state, returns nothing (or only an outcome of the change).
//    "Asking a question should not change the answer." — Bertrand Meyer

public class InvoiceNumberGenerator
{
    private int _next = 1000;

    public string PeekNextInvoiceNumber() => $"INV-{_next}";   // query

    public string ReserveInvoiceNumber() => $"INV-{_next++}";  // command (pragmatic: returns what it created)
}

public class ShoppingCart
{
    private readonly List<decimal> _items = [];
    private decimal _discount;
    private bool _welcomeDiscountApplied;

    public void Add(decimal price) => _items.Add(price);         // command

    public void ApplyWelcomeDiscount()                            // command
    {
        if (_welcomeDiscountApplied) return;
        _discount += 50m;
        _welcomeDiscountApplied = true;
    }

    public decimal Total => _items.Sum() - _discount;             // query
}

public class UserValidator
{
    private const int MaxFailedAttempts = 3;
    public int FailedAttempts { get; private set; }

    public bool IsLockedOut => FailedAttempts >= MaxFailedAttempts;   // query
    public bool Matches(string password) => password == "hunter2";    // query

    public bool TryLogin(string password)                             // command: name says it does something
    {
        if (IsLockedOut) return false;
        if (Matches(password)) return true;
        FailedAttempts++;
        return false;
    }
}
