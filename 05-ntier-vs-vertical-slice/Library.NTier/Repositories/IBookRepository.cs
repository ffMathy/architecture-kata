using Library.NTier.Models;

namespace Library.NTier.Repositories;

public interface IBookRepository
{
    IReadOnlyList<Book> GetAll();
    Book? GetById(int id);
}
