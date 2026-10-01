using Library.VerticalSlice.Infrastructure;

namespace Library.VerticalSlice.Features.Books;

// Everything about "list books" in one file: route, response shape, query.
// The response is tailored to THIS use case - no shared BookDto.
public static class ListBooks
{
    public record Response(int Id, string Title, bool IsAvailable);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/books", Handle);

    public static IEnumerable<Response> Handle(LibraryDb db) =>
        db.Books.Select(b => new Response(
            b.Id,
            b.Title,
            IsAvailable: !db.Loans.Any(l => l.BookId == b.Id && l.IsActive)));
}
