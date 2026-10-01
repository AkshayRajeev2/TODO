using DailyTaskTracker.Api.DTOs;
using DailyTaskTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskTracker.Api.Controllers;

[ApiController, Route("api/tasks")]
public class TasksController(ITaskService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<TaskResponse>>> Get(CancellationToken ct) => Ok(await service.GetTasksAsync(ct));
    [HttpPost] public async Task<ActionResult<TaskResponse>> Create(TaskCreateUpdateRequest request, CancellationToken ct) => Ok(await service.CreateAsync(request, ct));
    [HttpPut("{id:guid}")] public async Task<ActionResult<TaskResponse>> Update(Guid id, TaskCreateUpdateRequest request, CancellationToken ct) => Ok(await service.UpdateAsync(id, request, ct));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken ct) { await service.ArchiveAsync(id, ct); return NoContent(); }
    [HttpGet("{id:guid}/stats")] public async Task<ActionResult<TaskStatsResponse>> Stats(Guid id, CancellationToken ct) => Ok(await service.GetStatsAsync(id, ct));
}
