using DailyTaskTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskTracker.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskCompletion> TaskCompletions => Set<TaskCompletion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("tasks");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.RecurrenceType).HasConversion<int>();
            entity.Property(x => x.Color).HasMaxLength(20);
            entity.Property(x => x.StartDate).HasColumnType("date");
            entity.Property(x => x.EndDate).HasColumnType("date");
            entity.HasIndex(x => new { x.IsArchived, x.StartDate });
        });

        modelBuilder.Entity<TaskCompletion>(entity =>
        {
            entity.ToTable("task_completions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Date).HasColumnType("date");
            entity.HasIndex(x => new { x.TaskId, x.Date }).IsUnique();
            entity.HasOne(x => x.Task)
                .WithMany(x => x.Completions)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
