using Library.VerticalSlice.Domain;
using Library.VerticalSlice.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Library.VerticalSlice.Features.Loans;

public static class BorrowBook
{
    private const int MaxActiveLoansPerMember = 3;

    public record Request(int BookId, string MemberName);
    public record Response(int LoanId, int BookId, DateOnly DueOn);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/loans", Handle);

    public static Results<Ok<Response>, NotFound<string>, Conflict<string>> Handle(
        Request request, LibraryDb db, TimeProvider clock)
    {
        if (db.Books.All(b => b.Id != request.BookId))
            return TypedResults.NotFound($"Book {request.BookId} not found");

        if (db.Loans.Any(l => l.BookId == request.BookId && l.IsActive))
            return TypedResults.Conflict("Book is already on loan");

        if (db.Loans.Count(l => l.MemberName == request.MemberName && l.IsActive) >= MaxActiveLoansPerMember)
            return TypedResults.Conflict($"Members may have at most {MaxActiveLoansPerMember} active loans");

        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        var loan = new Loan(db.NextLoanId(), request.BookId, request.MemberName, today);
        db.Loans.Add(loan);

        return TypedResults.Ok(new Response(loan.Id, loan.BookId, loan.DueOn));
    }
}
