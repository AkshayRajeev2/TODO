using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyTaskTracker.Api.Migrations;

[DbContext(typeof(DailyTaskTracker.Api.Data.AppDbContext))]
[Migration("202609280001_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(name: "tasks", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
            RecurrenceType = table.Column<int>(type: "integer", nullable: false),
            DaysOfWeek = table.Column<int>(type: "integer", nullable: false),
            StartDate = table.Column<DateOnly>(type: "date", nullable: false),
            EndDate = table.Column<DateOnly>(type: "date", nullable: true),
            Color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            IsArchived = table.Column<bool>(type: "boolean", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        }, constraints: table => table.PrimaryKey("PK_tasks", x => x.Id));

        migrationBuilder.CreateTable(name: "task_completions", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            TaskId = table.Column<Guid>(type: "uuid", nullable: false),
            Date = table.Column<DateOnly>(type: "date", nullable: false),
            CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_task_completions", x => x.Id);
            table.ForeignKey("FK_task_completions_tasks_TaskId", x => x.TaskId, "tasks", "Id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateIndex(name: "IX_tasks_IsArchived_StartDate", table: "tasks", columns: new[] { "IsArchived", "StartDate" });
        migrationBuilder.CreateIndex(name: "IX_task_completions_TaskId_Date", table: "task_completions", columns: new[] { "TaskId", "Date" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "task_completions");
        migrationBuilder.DropTable(name: "tasks");
    }
}
