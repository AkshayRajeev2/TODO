using DailyTaskTracker.Api.DTOs;
using DailyTaskTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskTracker.Api.Controllers;

[ApiController, Route("api/days")]
public class DaysController(ITaskService service) : ControllerBase
{
    [HttpGet("{date}")] public async Task<ActionResult<DayResponse>> Get(DateOnly date, CancellationToken ct) => Ok(await service.GetDayAsync(date, ct));
    [HttpPut("{date}/tasks/{taskId:guid}/completion")]
    public async Task<IActionResult> Completion(DateOnly date, Guid taskId, CompletionRequest request, CancellationToken ct)
    { await service.SetCompletionAsync(date, taskId, request.Completed, ct); return NoContent(); }
}
