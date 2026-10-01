using Library.NTier.Dtos;

namespace Library.NTier.Services;

public interface IBookService
{
    IReadOnlyList<BookDto> GetAll();
    BookDto? GetById(int id);
}
