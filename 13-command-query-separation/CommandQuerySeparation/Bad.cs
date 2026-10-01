namespace CommandQuerySeparation.Bad;

// ❌ Methods that look like questions but secretly change state.

public class InvoiceNumberGenerator
{
    private int _next = 1000;

    // "Get" - sounds like a harmless read. It increments.
    // Call it in a log statement, a debugger watch, or a retry - and you burn numbers.
    public string GetNextInvoiceNumber() => $"INV-{_next++}";
}

public class ShoppingCart
{
    private readonly List<decimal> _items = [];
    private decimal? _cachedDiscount;

    public void Add(decimal price) => _items.Add(price);

    // A "query" that also applies a one-time welcome discount the first time it's asked.
    // Displaying the total in the UI now changes what the customer pays.
    public decimal GetTotal()
    {
        _cachedDiscount ??= 50m;
        var total = _items.Sum() - _cachedDiscount.Value;
        _cachedDiscount = 0; // welcome discount "used"
        return total;
    }
}

public class UserValidator
{
    public int FailedAttempts { get; private set; }

    // IsValid... and also locks you out after 3 calls.
    public bool IsValid(string password)
    {
        var ok = password == "hunter2";
        if (!ok) FailedAttempts++;
        return ok && FailedAttempts < 3;
    }
}
