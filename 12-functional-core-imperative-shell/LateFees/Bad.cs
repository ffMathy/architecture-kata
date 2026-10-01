namespace LateFees.Bad;

public class Loan
{
    public int Id { get; set; }
    public string MemberEmail { get; set; } = "";
    public DateTime DueDate { get; set; }
    public bool FeeCharged { get; set; }
}

public static class Database
{
    public static List<Loan> Loans { get; } =
    [
        new() { Id = 1, MemberEmail = "jane@example.com", DueDate = DateTime.Today.AddDays(-3) },
        new() { Id = 2, MemberEmail = "bob@example.com", DueDate = DateTime.Today.AddDays(-20) },
        new() { Id = 3, MemberEmail = "eve@example.com", DueDate = DateTime.Today.AddDays(5) },
    ];
}

// ❌ Business rules, I/O, time and side effects are tangled together.
//    How do you test "fees are capped at 200 kr"?
//      - You'd need a database with the right rows,
//      - DateTime.Today to cooperate,
//      - and to capture e-mails being sent.
//    So nobody writes that test.
public class LateFeeJob
{
    public void Run()
    {
        foreach (var loan in Database.Loans)                       // I/O
        {
            var daysLate = (DateTime.Today - loan.DueDate).Days;   // hidden input: the clock
            if (daysLate <= 0 || loan.FeeCharged) continue;

            if (daysLate < 7)
            {
                Console.WriteLine($"[SMTP] {loan.MemberEmail}: friendly reminder, loan {loan.Id} is {daysLate} days late");
            }
            else
            {
                var fee = Math.Min(200m, daysLate * 5m);           // the actual business rule, buried
                Console.WriteLine($"[Payments] charge {loan.MemberEmail} {fee} kr");
                Console.WriteLine($"[SMTP] {loan.MemberEmail}: you were charged {fee} kr for loan {loan.Id}");
                loan.FeeCharged = true;                            // mutation
            }
        }
    }
}
