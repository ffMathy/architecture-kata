namespace Library.NTier.Dtos;

public record LoanDto(int Id, int BookId, string MemberName, DateOnly BorrowedOn, DateOnly DueOn, DateOnly? ReturnedOn, decimal LateFee);
