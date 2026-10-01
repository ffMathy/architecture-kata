namespace TellDontAsk.Good;

public enum Plan { Free, Pro, Enterprise }

// ✅ TELL the object what you want. It decides, using its own data.
public class Subscription(Plan plan, DateOnly expiresOn, bool autoRenew, bool hasPaymentMethod)
{
    private bool _cancelled;
    private int _seatsUsed;

    public DateOnly ExpiresOn { get; private set; } = expiresOn;

    private bool IsDueForRenewal(DateOnly today) =>
        !_cancelled && autoRenew && hasPaymentMethod && plan != Plan.Free && ExpiresOn <= today;

    private int MaxSeats => plan switch { Plan.Free => 1, Plan.Pro => 10, _ => int.MaxValue };

    public bool RenewIfDue(DateOnly today)
    {
        if (!IsDueForRenewal(today)) return false;
        ExpiresOn = ExpiresOn.AddMonths(1);
        return true;
    }

    public bool TryAddSeat()
    {
        if (_seatsUsed >= MaxSeats) return false;
        _seatsUsed++;
        return true;
    }

    public void Cancel() => _cancelled = true;
}

public class RenewalJob
{
    public void Run(Subscription sub, DateOnly today)
    {
        if (sub.RenewIfDue(today))
            Console.WriteLine($"[RenewalJob] renewed until {sub.ExpiresOn:yyyy-MM-dd}");
    }
}

public class AdminController
{
    public void ForceRenew(Subscription sub, DateOnly today) =>
        Console.WriteLine(sub.RenewIfDue(today)
            ? $"[Admin] renewed until {sub.ExpiresOn:yyyy-MM-dd}"
            : "[Admin] not eligible for renewal ✅ (same rule as the job - there's only one copy)");
}
