namespace Encapsulation.Bad;

public class Transaction
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = "";
}

// ❌ An "anemic" class: just a bag of public getters and setters.
//    Invariants the business cares about:
//      1. Balance == sum of transactions
//      2. Balance never drops below -OverdraftLimit
//      3. A frozen account can't be debited
//    ...are enforced by nobody. Or rather: by every caller, hopefully, identically.
public class BankAccount
{
    public string Owner { get; set; } = "";
    public decimal Balance { get; set; }
    public decimal OverdraftLimit { get; set; } = 500;
    public bool IsFrozen { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
}

// The rules leak into "service" classes - and get duplicated.
public class AtmService
{
    public void Withdraw(BankAccount account, decimal amount)
    {
        if (account.IsFrozen) throw new InvalidOperationException("Frozen");
        if (account.Balance - amount < -account.OverdraftLimit) throw new InvalidOperationException("Insufficient funds");
        account.Balance -= amount;
        account.Transactions.Add(new Transaction { Amount = -amount, Description = "ATM" });
    }
}

public class MobilePayService
{
    // Written by another team six months later. Forgot the frozen check.
    // Forgot to record the transaction. Nobody noticed.
    public void Pay(BankAccount account, decimal amount)
    {
        if (account.Balance - amount < -account.OverdraftLimit) throw new InvalidOperationException("Insufficient funds");
        account.Balance -= amount;
    }
}
