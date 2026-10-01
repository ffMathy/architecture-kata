using Library.VerticalSlice.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Library.VerticalSlice.Features.Books;

public static class GetBook
{
    public record Response(int Id, string Title, string Author, bool IsAvailable, DateOnly? DueBackOn);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/books/{id:int}", Handle);

    public static Results<Ok<Response>, NotFound> Handle(int id, LibraryDb db)
    {
        var book = db.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return TypedResults.NotFound();

        var activeLoan = db.Loans.FirstOrDefault(l => l.BookId == id && l.IsActive);
        return TypedResults.Ok(new Response(book.Id, book.Title, book.Author, activeLoan is null, activeLoan?.DueOn));
    }
}
