namespace Library.VerticalSlice.Domain;

// Shared domain rules still live in the domain - slices don't mean "no domain model".
public class Loan
{
    public const int LoanPeriodDays = 14;
    private const decimal LateFeePerDay = 5m;

    public Loan(int id, int bookId, string memberName, DateOnly borrowedOn)
    {
        Id = id;
        BookId = bookId;
        MemberName = memberName;
        BorrowedOn = borrowedOn;
        DueOn = borrowedOn.AddDays(LoanPeriodDays);
    }

    public int Id { get; }
    public int BookId { get; }
    public string MemberName { get; }
    public DateOnly BorrowedOn { get; }
    public DateOnly DueOn { get; }
    public DateOnly? ReturnedOn { get; private set; }
    public bool IsActive => ReturnedOn is null;

    public decimal Return(DateOnly today)
    {
        if (!IsActive) throw new InvalidOperationException("Loan already returned");
        ReturnedOn = today;
        return Math.Max(0, today.DayNumber - DueOn.DayNumber) * LateFeePerDay;
    }
}
