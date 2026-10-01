using Library.NTier.Dtos;

namespace Library.NTier.Services;

public interface ILoanService
{
    LoanDto Borrow(BorrowBookRequest request);
    LoanDto Return(int loanId);
}
