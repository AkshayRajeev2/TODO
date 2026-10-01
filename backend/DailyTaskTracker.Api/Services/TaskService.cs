using DailyTaskTracker.Api.Data;
using DailyTaskTracker.Api.DTOs;
using DailyTaskTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskTracker.Api.Services;

public sealed class TaskService(AppDbContext db, IRecurrenceService recurrence) : ITaskService
{
    public async Task<IReadOnlyList<TaskResponse>> GetTasksAsync(CancellationToken ct) =>
        (await db.Tasks.AsNoTracking().Where(x => !x.IsArchived).OrderBy(x => x.Title).ToListAsync(ct))
            .Select(ToResponse).ToList();

    public async Task<TaskResponse> CreateAsync(TaskCreateUpdateRequest request, CancellationToken ct)
    {
        Validate(request);
        var task = new TaskItem
        {
            Id = Guid.NewGuid(), Title = request.Title.Trim(), Description = request.Description?.Trim(),
            RecurrenceType = request.RecurrenceType, DaysOfWeek = request.DaysOfWeek,
            StartDate = request.StartDate, EndDate = request.EndDate, Color = request.Color,
            CreatedAt = DateTime.UtcNow
        };
        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);
        return ToResponse(task);
    }

    public async Task<TaskResponse> UpdateAsync(Guid id, TaskCreateUpdateRequest request, CancellationToken ct)
    {
        Validate(request);
        var task = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsArchived, ct)
            ?? throw new KeyNotFoundException("Task not found.");
        task.Title = request.Title.Trim(); task.Description = request.Description?.Trim();
        task.RecurrenceType = request.RecurrenceType; task.DaysOfWeek = request.DaysOfWeek;
        task.StartDate = request.StartDate; task.EndDate = request.EndDate; task.Color = request.Color;
        await db.SaveChangesAsync(ct);
        return ToResponse(task);
    }

    public async Task ArchiveAsync(Guid id, CancellationToken ct)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && !x.IsArchived, ct)
            ?? throw new KeyNotFoundException("Task not found.");
        task.IsArchived = true;
        await db.SaveChangesAsync(ct);
    }

    public async Task<DayResponse> GetDayAsync(DateOnly date, CancellationToken ct)
    {
        var tasks = await db.Tasks.AsNoTracking().Where(x => !x.IsArchived && x.StartDate <= date && (!x.EndDate.HasValue || x.EndDate >= date)).ToListAsync(ct);
        var ids = tasks.Where(x => recurrence.IsDue(x, date)).Select(x => x.Id).ToArray();
        var completed = await db.TaskCompletions.AsNoTracking().Where(x => x.Date == date && ids.Contains(x.TaskId)).Select(x => x.TaskId).ToListAsync(ct);
        return new DayResponse(date, tasks.Where(x => ids.Contains(x.Id)).OrderBy(x => x.Title)
            .Select(x => new DayTaskResponse(x.Id, x.Title, x.Description, x.RecurrenceType, x.Color, completed.Contains(x.Id))).ToList());
    }

    public async Task SetCompletionAsync(DateOnly date, Guid taskId, bool completed, CancellationToken ct)
    {
        var task = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == taskId && !x.IsArchived, ct)
            ?? throw new KeyNotFoundException("Task not found.");
        if (date > DateOnly.FromDateTime(DateTime.Now)) throw new InvalidOperationException("Future tasks cannot be completed.");
        if (!recurrence.IsDue(task, date)) throw new InvalidOperationException("Task is not due on this date.");

        var existing = await db.TaskCompletions.FirstOrDefaultAsync(x => x.TaskId == taskId && x.Date == date, ct);
        if (completed && existing is null)
            db.TaskCompletions.Add(new TaskCompletion { Id = Guid.NewGuid(), TaskId = taskId, Date = date, CompletedAt = DateTime.UtcNow });
        else if (!completed && existing is not null)
            db.TaskCompletions.Remove(existing);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<CalendarDaySummary>> GetCalendarAsync(int year, int month, CancellationToken ct)
    {
        if (month is < 1 or > 12) throw new ArgumentException("Month must be between 1 and 12.");
        var first = new DateOnly(year, month, 1); var last = first.AddMonths(1).AddDays(-1);
        var tasks = await db.Tasks.AsNoTracking().Where(x => !x.IsArchived && x.StartDate <= last && (!x.EndDate.HasValue || x.EndDate >= first)).ToListAsync(ct);
        var completions = await db.TaskCompletions.AsNoTracking().Where(x => x.Date >= first && x.Date <= last).ToListAsync(ct);
        var completionLookup = completions.GroupBy(x => x.Date).ToDictionary(g => g.Key, g => g.Select(x => x.TaskId).ToHashSet());
        var result = new List<CalendarDaySummary>();
        for (var date = first; date <= last; date = date.AddDays(1))
        {
            var dueIds = tasks.Where(t => recurrence.IsDue(t, date)).Select(t => t.Id).ToHashSet();
            var done = completionLookup.TryGetValue(date, out var doneIds) ? dueIds.Intersect(doneIds).Count() : 0;
            result.Add(new CalendarDaySummary(date, dueIds.Count, done));
        }
        return result;
    }

    public async Task<TaskStatsResponse> GetStatsAsync(Guid taskId, CancellationToken ct)
    {
        var task = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == taskId && !x.IsArchived, ct)
            ?? throw new KeyNotFoundException("Task not found.");
        var today = DateOnly.FromDateTime(DateTime.Now); var from = today.AddDays(-29);
        var dueDates = recurrence.GetDates(task, from, today);
        var doneDates = (await db.TaskCompletions.AsNoTracking()
            .Where(x => x.TaskId == taskId && x.Date >= from && x.Date <= today)
            .Select(x => x.Date)
            .ToListAsync(ct))
            .ToHashSet();
        var completed = dueDates.Count(doneDates.Contains);
        var streak = 0;
        for (var date = today; date >= from; date = date.AddDays(-1))
        {
            if (!recurrence.IsDue(task, date)) continue;
            if (!doneDates.Contains(date)) break;
            streak++;
        }
        var rate = dueDates.Count == 0 ? 0m : Math.Round(completed * 100m / dueDates.Count, 1);
        return new TaskStatsResponse(task.Id, streak, dueDates.Count, completed, rate);
    }

    private static void Validate(TaskCreateUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title)) throw new ArgumentException("Title is required.");
        if (request.EndDate.HasValue && request.EndDate.Value < request.StartDate) throw new ArgumentException("End date cannot be before start date.");
        if (request.RecurrenceType == RecurrenceType.Weekly && request.DaysOfWeek is <= 0 or > 127) throw new ArgumentException("Weekly tasks require a valid DaysOfWeek bitmask.");
        if (request.RecurrenceType != RecurrenceType.Weekly && request.DaysOfWeek != 0) throw new ArgumentException("DaysOfWeek must be zero unless recurrence is Weekly.");
    }

    private static TaskResponse ToResponse(TaskItem x) => new(x.Id, x.Title, x.Description, x.RecurrenceType, x.DaysOfWeek, x.StartDate, x.EndDate, x.Color, x.IsArchived, x.CreatedAt);
}
