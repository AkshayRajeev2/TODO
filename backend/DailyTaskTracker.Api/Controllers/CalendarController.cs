using DailyTaskTracker.Api.DTOs;
using DailyTaskTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskTracker.Api.Controllers;

[ApiController, Route("api/calendar")]
public class CalendarController(ITaskService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<CalendarDaySummary>>> Get([FromQuery] int year, [FromQuery] int month, CancellationToken ct) => Ok(await service.GetCalendarAsync(year, month, ct));
}
