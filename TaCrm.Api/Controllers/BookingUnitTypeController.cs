using Microsoft.AspNetCore.Mvc;
using TaCrm.Common.Data;
using TaCrm.Common.Data.Dtos;
using TaCrm.Common.Handlers;

namespace ta_crm_api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class BookingUnitTypeController(
    BookingUnitTypeHandler bookingUnitTypeHandler
    ) : ControllerBase
{
    [HttpGet("GetBookableUnitTypes")]
    public async Task<ActionResult<IReadOnlyList<BookingUnitTypeEntity>>> GetBookableUnitTypes() =>
        new JsonResult(await bookingUnitTypeHandler.GetBookableUnitTypes(HttpContext.RequestAborted));

    [HttpGet("SearchAvailableUnits")]
    public async Task<ActionResult<IReadOnlyList<UnitPriceBand>>> SearchAvailableUnits([FromQuery] SearchUnitTypesSubmission submission)
    {
        return new JsonResult(await bookingUnitTypeHandler.SearchAvailableUnitTypes(submission, Request.HttpContext.RequestAborted));
    }
}