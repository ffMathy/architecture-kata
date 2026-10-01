using Library.NTier.Models;

namespace Library.NTier.Repositories;

public class InMemoryLoanRepository : ILoanRepository
{
    private readonly List<Loan> _loans = [];
    private int _nextId = 1;

    public Loan? GetById(int id) => _loans.FirstOrDefault(l => l.Id == id);

    public Loan? GetActiveLoanForBook(int bookId) =>
        _loans.FirstOrDefault(l => l.BookId == bookId && l.ReturnedOn is null);

    public int CountActiveLoansForMember(string memberName) =>
        _loans.Count(l => l.MemberName == memberName && l.ReturnedOn is null);

    public void Add(Loan loan)
    {
        loan.Id = _nextId++;
        _loans.Add(loan);
    }

    public void Update(Loan loan) { /* in-memory: already updated by reference */ }
}
