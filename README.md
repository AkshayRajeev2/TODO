# Daily Task Tracker

A complete daily task tracking application with:

- **ASP.NET Core Web API (.NET 8)**
- **EF Core 8 + Npgsql**
- **PostgreSQL 16**
- **Angular 17 standalone components + signals**
- **Angular Material**
- **xUnit recurrence/calendar tests**

## Project structure

```text
daily-task-tracker/
├── backend/
│   ├── DailyTaskTracker.sln
│   ├── DailyTaskTracker.Api/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── DTOs/
│   │   ├── Middleware/
│   │   ├── Migrations/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Properties/launchSettings.json
│   │   ├── DailyTaskTracker.Api.csproj
│   │   ├── Program.cs
│   │   ├── Dockerfile
│   │   └── appsettings*.json
│   └── DailyTaskTracker.Tests/
│       ├── RecurrenceServiceTests.cs
│       └── DailyTaskTracker.Tests.csproj
├── frontend/
│   └── daily-task-tracker/
│       ├── src/app/core/
│       ├── src/app/features/today/
│       ├── src/app/features/tasks/
│       ├── src/app/features/calendar/
│       ├── src/app/shared/
│       ├── package.json
│       ├── angular.json
│       └── Dockerfile
├── docker-compose.yml
├── docker-compose.full.yml
└── README.md
```

## Prerequisites

### Local development

- .NET 8 SDK
- Node.js 20.x recommended
- npm 10+
- Angular CLI 17.x (optional globally; `npx ng` also works)
- Docker Desktop / Docker Engine for PostgreSQL

## 1. Start PostgreSQL

From the repository root:

```bash
docker compose up -d postgres
```

The database is exposed at `localhost:5432` with:

```text
Database: daily_task_tracker
Username: postgres
Password: postgres
```

For local development the API connection string is in `backend/DailyTaskTracker.Api/appsettings.Development.json`.

## 2. Run EF Core migrations

Install the EF CLI if necessary:

```bash
dotnet tool install --global dotnet-ef --version 8.*
```

Then:

```bash
cd backend
 dotnet restore DailyTaskTracker.sln
 dotnet ef database update --project DailyTaskTracker.Api --startup-project DailyTaskTracker.Api
```

If you are starting from a clean database, the repository includes the initial migration `202609280001_InitialCreate`.

To create future migrations:

```bash
dotnet ef migrations add YourMigrationName --project DailyTaskTracker.Api --startup-project DailyTaskTracker.Api
```

## 3. Run the API

```bash
cd backend
 dotnet run --project DailyTaskTracker.Api
```

The configured development URL is:

```text
http://localhost:5080
```

Swagger:

```text
http://localhost:5080/swagger
```

## 4. Run the Angular app

```bash
cd frontend/daily-task-tracker
npm install
npm start
```

Open:

```text
http://localhost:4200
```

The Angular development app calls `http://localhost:5080/api`.

## 5. Run tests

```bash
cd backend
dotnet test DailyTaskTracker.sln
```

The tests cover:

- Daily recurrence boundaries
- Weekly weekday bitmask behavior
- One-time recurrence
- End-date handling
- Calendar due/completed calculation

## Full Docker stack

The primary `docker-compose.yml` intentionally runs only PostgreSQL so developers can run and debug the API/Angular applications locally.

For an all-container setup:

```bash
docker compose -f docker-compose.full.yml up --build
```

Then use:

- Frontend: `http://localhost:4200`
- API: `http://localhost:5080`

**Note:** the full stack starts the API but does not automatically execute EF migrations. Run the migration once against the PostgreSQL container, or add a dedicated migration/init container in production infrastructure.

## API endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/api/tasks` | Active task list |
| POST | `/api/tasks` | Create task |
| PUT | `/api/tasks/{id}` | Update task |
| DELETE | `/api/tasks/{id}` | Archive task |
| GET | `/api/days/{date}` | Tasks due on a date + completion state |
| PUT | `/api/days/{date}/tasks/{taskId}/completion` | Set completion |
| GET | `/api/calendar?year=&month=` | Monthly completion summary |
| GET | `/api/tasks/{id}/stats` | 30-day stats and current streak |

Example completion payload:

```json
{ "completed": true }
```

## Recurrence representation

`DaysOfWeek` is an integer bitmask:

| Day | Bit |
|---|---:|
| Monday | 1 |
| Tuesday | 2 |
| Wednesday | 4 |
| Thursday | 8 |
| Friday | 16 |
| Saturday | 32 |
| Sunday | 64 |

For example, Monday + Wednesday + Friday = `1 + 4 + 16 = 21`.

## Architecture notes

### Backend

- Controllers handle HTTP concerns only.
- `TaskService` owns task/day/calendar/stat application behavior.
- `RecurrenceService` is isolated so recurrence rules can be unit-tested independently.
- `AppDbContext` contains PostgreSQL/EF mapping.
- A global middleware converts expected exceptions into a consistent JSON error shape.
- `DateOnly` is used for all task dates and completion dates; completion timestamps are UTC.
- No authentication is included. The service/API boundaries can later add a `UserId` to `TaskItem`, completion queries, and DTO authorization without changing recurrence semantics.

### Frontend

- Standalone Angular components.
- Signals are used for local UI state and derived values.
- `TaskApiService` provides typed HTTP operations.
- Reactive forms drive the task editor.
- Completion toggles update the UI optimistically and roll back when the API request fails.
- Calendar cells expose ARIA labels and keyboard-operable buttons.
- Future dates can be viewed but their completion checkbox is disabled; the backend also rejects future completion attempts.

## Assumptions

1. This is a single-user application initially; authentication and `UserId` scoping are intentionally omitted.
2. The server interprets `DateOnly` as a calendar date. The client sends its local date rather than an ISO timestamp.
3. A task's current streak counts consecutive **due dates** completed, skipping days on which a weekly task was not due.
4. The 30-day completion rate is `completed due instances / due instances * 100` for today and the preceding 29 calendar days.
5. Archived tasks disappear from active task lists and future calendar/day calculations, while historical completion rows are retained.
6. One-time tasks are due only on their `StartDate`.
7. Colors are optional CSS-compatible color strings; no server-side color palette is enforced.

## Suggested next steps

- JWT/OIDC authentication and `UserId` data isolation.
- Notifications/reminders using a background worker and email/push integration.
- Tags/projects and filtering.
- Task notes/attachments.
- CSV/JSON export and import.
- Time-zone preference per user if reminders are added.
- Integration tests using Testcontainers for PostgreSQL.
- Production migration/init job and secrets management.
- E2E tests with Playwright.
