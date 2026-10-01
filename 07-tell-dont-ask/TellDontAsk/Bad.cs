namespace TellDontAsk.Bad;

public enum Plan { Free, Pro, Enterprise }

// ❌ The object is a data holder. Callers ASK for its state, make the decision,
//    then reach back in to change it.
public class Subscription
{
    public Plan Plan { get; set; }
    public DateOnly ExpiresOn { get; set; }
    public bool AutoRenew { get; set; }
    public bool HasPaymentMethod { get; set; }
    public bool IsCancelled { get; set; }
    public int SeatsUsed { get; set; }
}

public class RenewalJob
{
    public void Run(Subscription sub, DateOnly today)
    {
        // Ask, ask, ask, ask, ask... decide... then poke.
        if (!sub.IsCancelled && sub.AutoRenew && sub.HasPaymentMethod && sub.ExpiresOn <= today && sub.Plan != Plan.Free)
        {
            sub.ExpiresOn = sub.ExpiresOn.AddMonths(1);
            Console.WriteLine($"[RenewalJob] renewed until {sub.ExpiresOn:yyyy-MM-dd}");
        }
    }
}

public class AdminController
{
    // The same rule, copied - but subtly different. Spot the bug?
    public void ForceRenew(Subscription sub, DateOnly today)
    {
        if (sub.AutoRenew && sub.HasPaymentMethod && sub.ExpiresOn <= today)
        {
            sub.ExpiresOn = sub.ExpiresOn.AddMonths(1);
            Console.WriteLine($"[Admin] renewed until {sub.ExpiresOn:yyyy-MM-dd}  <- renewed a CANCELLED, FREE plan 💥");
        }
    }
}

public class TeamController
{
    public void AddSeat(Subscription sub)
    {
        var maxSeats = sub.Plan switch { Plan.Free => 1, Plan.Pro => 10, _ => int.MaxValue };
        if (sub.SeatsUsed < maxSeats) sub.SeatsUsed++;
    }
}
