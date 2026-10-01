namespace Library.NTier.Models;

public class Loan
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string MemberName { get; set; } = "";
    public DateOnly BorrowedOn { get; set; }
    public DateOnly DueOn { get; set; }
    public DateOnly? ReturnedOn { get; set; }
}
