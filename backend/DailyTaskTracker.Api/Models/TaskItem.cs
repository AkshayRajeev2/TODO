namespace DailyTaskTracker.Api.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public int DaysOfWeek { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Color { get; set; }
    public bool IsArchived { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<TaskCompletion> Completions { get; set; } = new List<TaskCompletion>();
}
