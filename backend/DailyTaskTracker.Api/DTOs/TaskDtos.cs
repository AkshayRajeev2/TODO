using DailyTaskTracker.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace DailyTaskTracker.Api.DTOs;

public sealed class TaskCreateUpdateRequest
{
    [Required, MaxLength(200)] public string Title { get; init; } = string.Empty;
    [MaxLength(2000)] public string? Description { get; init; }
    [Required] public RecurrenceType RecurrenceType { get; init; }
    public int DaysOfWeek { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    [MaxLength(20)] public string? Color { get; init; }
}

public sealed record TaskResponse(
    Guid Id, string Title, string? Description, RecurrenceType RecurrenceType,
    int DaysOfWeek, DateOnly StartDate, DateOnly? EndDate, string? Color,
    bool IsArchived, DateTime CreatedAt);

public sealed record DayTaskResponse(
    Guid Id, string Title, string? Description, RecurrenceType RecurrenceType,
    string? Color, bool Completed);

public sealed record DayResponse(DateOnly Date, IReadOnlyList<DayTaskResponse> Tasks);

public sealed record CalendarDaySummary(DateOnly Date, int DueCount, int CompletedCount);

public sealed class CompletionRequest
{
    public bool Completed { get; init; }
}

public sealed record TaskStatsResponse(Guid TaskId, int CurrentStreak, int Last30DaysDue, int Last30DaysCompleted, decimal Last30DaysCompletionRate);
