using DailyTaskTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DailyTaskTracker.Api.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.20");
        modelBuilder.HasAnnotation("Relational:MaxIdentifierLength", 63);

        modelBuilder.Entity("DailyTaskTracker.Api.Models.TaskCompletion", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<DateTime>("CompletedAt").HasColumnType("timestamp with time zone");
            b.Property<DateOnly>("Date").HasColumnType("date");
            b.Property<Guid>("TaskId").HasColumnType("uuid");
            b.HasKey("Id");
            b.HasIndex("TaskId", "Date").IsUnique();
            b.ToTable("task_completions");
        });
        modelBuilder.Entity("DailyTaskTracker.Api.Models.TaskItem", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.Property<int>("DaysOfWeek").HasColumnType("integer");
            b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");
            b.Property<string>("Color").HasMaxLength(20).HasColumnType("character varying(20)");
            b.Property<string>("Description").HasMaxLength(2000).HasColumnType("character varying(2000)");
            b.Property<DateOnly?>("EndDate").HasColumnType("date");
            b.Property<bool>("IsArchived").HasColumnType("boolean");
            b.Property<DateOnly>("StartDate").HasColumnType("date");
            b.Property<string>("Title").IsRequired().HasMaxLength(200).HasColumnType("character varying(200)");
            b.Property<int>("RecurrenceType").HasColumnType("integer");
            b.HasKey("Id");
            b.HasIndex("IsArchived", "StartDate");
            b.ToTable("tasks");
        });
        modelBuilder.Entity("DailyTaskTracker.Api.Models.TaskCompletion", b =>
        {
            b.HasOne("DailyTaskTracker.Api.Models.TaskItem", "Task")
                .WithMany("Completions")
                .HasForeignKey("TaskId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            b.Navigation("Task");
        });
        modelBuilder.Entity("DailyTaskTracker.Api.Models.TaskItem", b => b.Navigation("Completions"));
    }
}
