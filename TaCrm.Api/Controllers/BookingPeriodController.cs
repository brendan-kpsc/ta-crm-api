using Microsoft.AspNetCore.Mvc;
using TaCrm.Common.Data;
using TaCrm.Common.Handlers;

namespace ta_crm_api.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingPeriodController 
    (BookingPeriodHandler bookingPeriodHandler): ControllerBase
{
    [HttpGet("GetAllBookingPeriods")]
    public async Task<ActionResult<IReadOnlyList<BookingPeriodEntity>>> GetAllBookingPeriods() =>
        new JsonResult(await bookingPeriodHandler.GetAllBookingPeriods(Request.HttpContext.RequestAborted));
}