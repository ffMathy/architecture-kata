using Library.NTier.Models;

namespace Library.NTier.Repositories;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books =
    [
        new() { Id = 1, Title = "Domain-Driven Design", Author = "Eric Evans" },
        new() { Id = 2, Title = "Refactoring", Author = "Martin Fowler" },
        new() { Id = 3, Title = "Clean Architecture", Author = "Robert C. Martin" },
        new() { Id = 4, Title = "A Philosophy of Software Design", Author = "John Ousterhout" },
    ];

    public IReadOnlyList<Book> GetAll() => _books;
    public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);
}
