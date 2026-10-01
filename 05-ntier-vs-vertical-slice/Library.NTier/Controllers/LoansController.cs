using Library.NTier.Dtos;
using Library.NTier.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.NTier.Controllers;

[ApiController]
[Route("loans")]
public class LoansController(ILoanService loanService) : ControllerBase
{
    [HttpPost]
    public ActionResult<LoanDto> Borrow(BorrowBookRequest request)
    {
        try
        {
            return Ok(loanService.Borrow(request));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }

    [HttpPost("{id:int}/return")]
    public ActionResult<LoanDto> Return(int id)
    {
        try
        {
            return Ok(loanService.Return(id));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }
}
