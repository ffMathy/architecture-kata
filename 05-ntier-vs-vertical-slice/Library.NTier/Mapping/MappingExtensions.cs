using Library.NTier.Dtos;
using Library.NTier.Models;

namespace Library.NTier.Mapping;

public static class MappingExtensions
{
    public static BookDto ToDto(this Book book, Loan? activeLoan) =>
        new(book.Id, book.Title, book.Author, activeLoan is null, activeLoan?.DueOn);

    public static LoanDto ToDto(this Loan loan, decimal lateFee) =>
        new(loan.Id, loan.BookId, loan.MemberName, loan.BorrowedOn, loan.DueOn, loan.ReturnedOn, lateFee);
}
