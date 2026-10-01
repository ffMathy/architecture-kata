namespace Encapsulation.Good;

public record Transaction(decimal Amount, string Description, DateTimeOffset At);

// ✅ The object owns its data AND the rules about that data.
//    The only way to change state is through methods that keep it valid.
public class BankAccount
{
    private readonly List<Transaction> _transactions = [];

    public BankAccount(string owner, decimal overdraftLimit = 500)
    {
        Owner = owner;
        OverdraftLimit = overdraftLimit;
    }

    public string Owner { get; }

    // C# 14 `field` keyword: validation without a hand-written backing field.
    public decimal OverdraftLimit
    {
        get;
        private set => field = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    public bool IsFrozen { get; private set; }

    // Derived, so it can never disagree with the transactions.
    public decimal Balance => _transactions.Sum(t => t.Amount);

    // Callers can read history but can't add, remove or cast it back to List<T>.
    public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

    public void Deposit(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Deposit must be positive");
        _transactions.Add(new(amount, description, DateTimeOffset.UtcNow));
    }

    public void Withdraw(decimal amount, string description)
    {
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Withdrawal must be positive");
        if (IsFrozen) throw new InvalidOperationException("Account is frozen");
        if (Balance - amount < -OverdraftLimit) throw new InvalidOperationException("Insufficient funds");
        _transactions.Add(new(-amount, description, DateTimeOffset.UtcNow));
    }

    public void Freeze() => IsFrozen = true;
}

// Services become thin. They *can't* forget a rule because they don't implement any.
public class AtmService
{
    public void Withdraw(BankAccount account, decimal amount) => account.Withdraw(amount, "ATM");
}

public class MobilePayService
{
    public void Pay(BankAccount account, decimal amount) => account.Withdraw(amount, "MobilePay");
}
