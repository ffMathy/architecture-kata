using Library.NTier.Dtos;
using Library.NTier.Mapping;
using Library.NTier.Repositories;

namespace Library.NTier.Services;

// Mostly a pass-through: controller -> service -> repository -> back up again.
public class BookService(IBookRepository books, ILoanRepository loans) : IBookService
{
    public IReadOnlyList<BookDto> GetAll() =>
        books.GetAll().Select(b => b.ToDto(loans.GetActiveLoanForBook(b.Id))).ToList();

    public BookDto? GetById(int id)
    {
        var book = books.GetById(id);
        return book?.ToDto(loans.GetActiveLoanForBook(book.Id));
    }
}
