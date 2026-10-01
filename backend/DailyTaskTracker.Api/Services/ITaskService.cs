using DailyTaskTracker.Api.DTOs;

namespace DailyTaskTracker.Api.Services;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> GetTasksAsync(CancellationToken ct);
    Task<TaskResponse> CreateAsync(TaskCreateUpdateRequest request, CancellationToken ct);
    Task<TaskResponse> UpdateAsync(Guid id, TaskCreateUpdateRequest request, CancellationToken ct);
    Task ArchiveAsync(Guid id, CancellationToken ct);
    Task<DayResponse> GetDayAsync(DateOnly date, CancellationToken ct);
    Task SetCompletionAsync(DateOnly date, Guid taskId, bool completed, CancellationToken ct);
    Task<IReadOnlyList<CalendarDaySummary>> GetCalendarAsync(int year, int month, CancellationToken ct);
    Task<TaskStatsResponse> GetStatsAsync(Guid taskId, CancellationToken ct);
}
