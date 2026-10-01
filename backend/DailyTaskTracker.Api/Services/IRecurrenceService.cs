using DailyTaskTracker.Api.Models;

namespace DailyTaskTracker.Api.Services;

public interface IRecurrenceService
{
    bool IsDue(TaskItem task, DateOnly date);
    IReadOnlyList<DateOnly> GetDates(TaskItem task, DateOnly from, DateOnly to);
}
