using Library.NTier.Models;

namespace Library.NTier.Repositories;

// This interface grows a method for every query any service ever needs.
public interface ILoanRepository
{
    Loan? GetById(int id);
    Loan? GetActiveLoanForBook(int bookId);
    int CountActiveLoansForMember(string memberName);
    void Add(Loan loan);
    void Update(Loan loan);
}
