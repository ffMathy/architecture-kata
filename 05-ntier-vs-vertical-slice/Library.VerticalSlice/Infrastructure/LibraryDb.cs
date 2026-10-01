using Library.VerticalSlice.Domain;

namespace Library.VerticalSlice.Infrastructure;

/// <summary>
/// Stand-in for an EF Core DbContext. Slices query it directly -
/// no generic repository layer in between.
/// </summary>
public class LibraryDb
{
    private int _nextLoanId = 1;

    public List<Book> Books { get; } =
    [
        new(1, "Domain-Driven Design", "Eric Evans"),
        new(2, "Refactoring", "Martin Fowler"),
        new(3, "Clean Architecture", "Robert C. Martin"),
        new(4, "A Philosophy of Software Design", "John Ousterhout"),
    ];

    public List<Loan> Loans { get; } = [];

    public int NextLoanId() => _nextLoanId++;
}
