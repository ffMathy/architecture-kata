using Library.VerticalSlice.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Library.VerticalSlice.Features.Loans;

public static class ReturnBook
{
    public record Response(int LoanId, DateOnly ReturnedOn, decimal LateFee);

    public static void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/loans/{id:int}/return", Handle);

    public static Results<Ok<Response>, NotFound<string>, Conflict<string>> Handle(
        int id, LibraryDb db, TimeProvider clock)
    {
        var loan = db.Loans.FirstOrDefault(l => l.Id == id);
        if (loan is null) return TypedResults.NotFound($"Loan {id} not found");
        if (!loan.IsActive) return TypedResults.Conflict("Loan already returned");

        var today = DateOnly.FromDateTime(clock.GetLocalNow().Date);
        var lateFee = loan.Return(today);

        return TypedResults.Ok(new Response(loan.Id, today, lateFee));
    }
}
