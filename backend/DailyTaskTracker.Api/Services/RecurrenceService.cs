using DailyTaskTracker.Api.Models;

namespace DailyTaskTracker.Api.Services;

public sealed class RecurrenceService : IRecurrenceService
{
    public bool IsDue(TaskItem task, DateOnly date)
    {
        if (task.IsArchived || date < task.StartDate || (task.EndDate.HasValue && date > task.EndDate.Value)) return false;
        return task.RecurrenceType switch
        {
            RecurrenceType.Daily => true,
            RecurrenceType.Weekly => (task.DaysOfWeek & BitFor(date.DayOfWeek)) != 0,
            RecurrenceType.OneTime => date == task.StartDate,
            _ => false
        };
    }

    public IReadOnlyList<DateOnly> GetDates(TaskItem task, DateOnly from, DateOnly to)
    {
        if (to < from) return [];
        var start = from > task.StartDate ? from : task.StartDate;
        var end = task.EndDate.HasValue && task.EndDate.Value < to ? task.EndDate.Value : to;
        if (end < start) return [];

        if (task.RecurrenceType == RecurrenceType.OneTime)
            return task.StartDate >= from && task.StartDate <= to ? [task.StartDate] : [];

        var dates = new List<DateOnly>();
        for (var date = start; date <= end; date = date.AddDays(1))
            if (IsDue(task, date)) dates.Add(date);
        return dates;
    }

    public static int BitFor(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => 1,
        DayOfWeek.Tuesday => 2,
        DayOfWeek.Wednesday => 4,
        DayOfWeek.Thursday => 8,
        DayOfWeek.Friday => 16,
        DayOfWeek.Saturday => 32,
        DayOfWeek.Sunday => 64,
        _ => 0
    };
}
