namespace DailyTaskTracker.Api.Models;

public class TaskCompletion
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public DateOnly Date { get; set; }
    public DateTime CompletedAt { get; set; }
    public TaskItem Task { get; set; } = null!;
}
