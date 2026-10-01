using DailyTaskTracker.Api.Models;
using DailyTaskTracker.Api.Services;
using Xunit;

namespace DailyTaskTracker.Tests;

public class RecurrenceServiceTests
{
    private readonly RecurrenceService sut = new();

    [Fact]
    public void Daily_IsDue_WithinDateRange()
    {
        var task = new TaskItem { StartDate = new DateOnly(2026, 9, 1), RecurrenceType = RecurrenceType.Daily };
        Assert.True(sut.IsDue(task, new DateOnly(2026, 9, 28)));
        Assert.False(sut.IsDue(task, new DateOnly(2026, 8, 31)));
    }

    [Fact]
    public void Weekly_UsesMondayBasedBitmask()
    {
        var task = new TaskItem { StartDate = new DateOnly(2026, 9, 1), RecurrenceType = RecurrenceType.Weekly, DaysOfWeek = 1 | 4 };
        Assert.True(sut.IsDue(task, new DateOnly(2026, 9, 7))); // Monday
        Assert.True(sut.IsDue(task, new DateOnly(2026, 9, 9))); // Wednesday
        Assert.False(sut.IsDue(task, new DateOnly(2026, 9, 8))); // Tuesday
    }

    [Fact]
    public void OneTime_IsDue_OnlyOnStartDate()
    {
        var task = new TaskItem { StartDate = new DateOnly(2026, 9, 28), RecurrenceType = RecurrenceType.OneTime };
        Assert.True(sut.IsDue(task, new DateOnly(2026, 9, 28)));
        Assert.False(sut.IsDue(task, new DateOnly(2026, 9, 29)));
    }

    [Fact]
    public void EndDate_StopsRecurrence()
    {
        var task = new TaskItem { StartDate = new DateOnly(2026, 9, 1), EndDate = new DateOnly(2026, 9, 10), RecurrenceType = RecurrenceType.Daily };
        Assert.True(sut.IsDue(task, new DateOnly(2026, 9, 10)));
        Assert.False(sut.IsDue(task, new DateOnly(2026, 9, 11)));
    }
}

public class CalendarSummaryCalculationTests
{
    [Fact]
    public void Summary_CountsOnlyDueAndCompletedTasks()
    {
        var recurrence = new RecurrenceService();
        var date = new DateOnly(2026, 9, 28);
        var tasks = new[]
        {
            new TaskItem { Id = Guid.NewGuid(), StartDate = date, RecurrenceType = RecurrenceType.Daily },
            new TaskItem { Id = Guid.NewGuid(), StartDate = date, RecurrenceType = RecurrenceType.Daily },
            new TaskItem { Id = Guid.NewGuid(), StartDate = date, RecurrenceType = RecurrenceType.OneTime }
        };
        var completed = new HashSet<Guid> { tasks[0].Id, Guid.NewGuid() };
        var due = tasks.Count(t => recurrence.IsDue(t, date));
        var done = tasks.Count(t => recurrence.IsDue(t, date) && completed.Contains(t.Id));
        Assert.Equal(3, due);
        Assert.Equal(1, done);
    }
}
