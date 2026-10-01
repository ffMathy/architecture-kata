using Library.NTier.Dtos;
using Library.NTier.Mapping;
using Library.NTier.Models;
using Library.NTier.Repositories;

namespace Library.NTier.Services;

// The "LoanService" collects every loan-related use case. Over time it becomes
// the 2.000-line class everybody edits (see 04-low-cohesion).
public class LoanService(IBookRepository books, ILoanRepository loans, TimeProvider clock) : ILoanService
{
    private const int MaxActiveLoansPerMember = 3;
    private const int LoanPeriodDays = 14;
    private const decimal LateFeePerDay = 5m;

    public LoanDto Borrow(BorrowBookRequest request)
    {
        _ = books.GetById(request.BookId) ?? throw new KeyNotFoundException($"Book {request.BookId} not found");

        if (loans.GetActiveLoanForBook(request.BookId) is not null)
            throw new InvalidOperationException("Book is already on loan");

        if (loans.CountActiveLoansForMember(request.MemberName) >= MaxActiveLoansPerMember)
            throw new InvalidOperationException($"Members may have at most {MaxActiveLoansPerMember} active loans");

        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        var loan = new Loan
        {
            BookId = request.BookId,
            MemberName = request.MemberName,
            BorrowedOn = today,
            DueOn = today.AddDays(LoanPeriodDays),
        };
        loans.Add(loan);
        return loan.ToDto(lateFee: 0);
    }

    public LoanDto Return(int loanId)
    {
        var loan = loans.GetById(loanId) ?? throw new KeyNotFoundException($"Loan {loanId} not found");
        if (loan.ReturnedOn is not null)
            throw new InvalidOperationException("Loan already returned");

        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        loan.ReturnedOn = today;
        loans.Update(loan);

        var daysLate = Math.Max(0, today.DayNumber - loan.DueOn.DayNumber);
        return loan.ToDto(daysLate * LateFeePerDay);
    }
}
