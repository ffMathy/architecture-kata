using Library.NTier.Dtos;
using Library.NTier.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.NTier.Controllers;

[ApiController]
[Route("books")]
public class BooksController(IBookService bookService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyList<BookDto>> GetAll() => Ok(bookService.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<BookDto> GetById(int id) =>
        bookService.GetById(id) is { } book ? Ok(book) : NotFound();
}
