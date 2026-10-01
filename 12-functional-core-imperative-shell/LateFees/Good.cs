namespace LateFees.Good;

// =====================================================================
//  FUNCTIONAL CORE - pure: data in, decisions out. No I/O, no clock,
//  no mutation. Trivial to test: see ../LateFees.Tests
// =====================================================================

public record Loan(int Id, string MemberEmail, DateOnly DueOn, bool FeeCharged);

public abstract record Decision(int LoanId, string MemberEmail);
public sealed record SendReminder(int LoanId, string MemberEmail, int DaysLate) : Decision(LoanId, MemberEmail);
public sealed record ChargeFee(int LoanId, string MemberEmail, decimal Amount) : Decision(LoanId, MemberEmail);

public static class LateFeePolicy
{
    public const int GraceDays = 7;
    public const decimal FeePerDay = 5m;
    public const decimal MaxFee = 200m;

    public static IReadOnlyList<Decision> Decide(IEnumerable<Loan> loans, DateOnly today) =>
        loans.Select(loan => Decide(loan, today)).OfType<Decision>().ToList();

    public static Decision? Decide(Loan loan, DateOnly today)
    {
        var daysLate = today.DayNumber - loan.DueOn.DayNumber;

        if (daysLate <= 0 || loan.FeeCharged) return null;
        if (daysLate < GraceDays) return new SendReminder(loan.Id, loan.MemberEmail, daysLate);
        return new ChargeFee(loan.Id, loan.MemberEmail, Math.Min(MaxFee, daysLate * FeePerDay));
    }
}

// =====================================================================
//  IMPERATIVE SHELL - thin, boring, no decisions. Gathers inputs,
//  calls the core, executes the outputs.
// =====================================================================

public interface ILoanStore
{
    IReadOnlyList<Loan> GetOpenLoans();
    void MarkFeeCharged(int loanId);
}

public interface IMailer { void Send(string to, string text); }
public interface IPayments { void Charge(string email, decimal amount); }

public class LateFeeJob(ILoanStore store, IMailer mailer, IPayments payments, TimeProvider clock)
{
    public void Run()
    {
        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        var decisions = LateFeePolicy.Decide(store.GetOpenLoans(), today);

        foreach (var decision in decisions)
        {
            switch (decision)
            {
                case SendReminder r:
                    mailer.Send(r.MemberEmail, $"friendly reminder, loan {r.LoanId} is {r.DaysLate} days late");
                    break;
                case ChargeFee c:
                    payments.Charge(c.MemberEmail, c.Amount);
                    mailer.Send(c.MemberEmail, $"you were charged {c.Amount} kr for loan {c.LoanId}");
                    store.MarkFeeCharged(c.LoanId);
                    break;
            }
        }
    }
}

// ---- Adapters used by Program.cs ------------------------------------

public class InMemoryLoanStore(List<Loan> loans) : ILoanStore
{
    public IReadOnlyList<Loan> GetOpenLoans() => loans;
    public void MarkFeeCharged(int loanId)
    {
        var i = loans.FindIndex(l => l.Id == loanId);
        loans[i] = loans[i] with { FeeCharged = true };
    }
}

public class ConsoleMailer : IMailer
{
    public void Send(string to, string text) => Console.WriteLine($"[SMTP] {to}: {text}");
}

public class ConsolePayments : IPayments
{
    public void Charge(string email, decimal amount) => Console.WriteLine($"[Payments] charge {email} {amount} kr");
}
