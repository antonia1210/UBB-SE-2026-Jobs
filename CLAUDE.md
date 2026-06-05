# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Solution layout

Five projects, wired together via `UBB_SE_2026_Jobs.slnx` (new-format XML solution; there is no `.sln`). All target **.NET 10**.

| Project | Type | Role |
|---------|------|------|
| `UBB_SE_2026_Jobs.Library` | classlib | Shared core: domain models, EF Core `JobsDbContext`, repositories, services, service proxies, mappers, DTOs, validators, migrations. Referenced by every other project. |
| `UBB_SE_2026_Jobs.Api` | ASP.NET Core Web API | REST backend. JWT auth, controllers, auto-applies EF migrations on startup. Hosts OpenAPI/Scalar in Development. |
| `UBB_SE_2026_Jobs.App` | WinUI 3 (Windows App SDK) desktop client | MVVM UI. Talks to the Api over HTTP via *service proxies* (never touches the DB directly). x64. |
| `UBB_SE_2026_Jobs.Web` | ASP.NET Core MVC | Browser client (skeleton — default `Home/Index` routing). |
| `UBB_SE_2026_Jobs.Tests` | xUnit | Tests against the Library. |

The README describes running Api + App + Web together as **Multiple Startup Projects** in Visual Studio (set Library and Tests to *None*).

## Build / run / test

```powershell
# Build the whole solution
dotnet build UBB_SE_2026_Jobs.slnx

# Run the API (auto-migrates the DB on startup; serves Scalar UI at /scalar in Dev)
dotnet run --project UBB_SE_2026_Jobs.Api

# Run the Web MVC app
dotnet run --project UBB_SE_2026_Jobs.Web

# Run all tests
dotnet test UBB_SE_2026_Jobs.Tests

# Run a single test class or method (xUnit, via VSTest filter)
dotnet test UBB_SE_2026_Jobs.Tests --filter "FullyQualifiedName~MyTestClass"
dotnet test UBB_SE_2026_Jobs.Tests --filter "FullyQualifiedName~MyTestClass.MyTestMethod"
```

The **App** is a WinUI 3 / MSIX project and must build for a concrete platform (x64); it is normally built and launched from Visual Studio rather than `dotnet run`. Building the App from the CLI requires `-p:Platform=x64`.

### EF Core migrations

The `DbContext` (`JobsDbContext`) and the `Migrations/` folder live in the **Library**, but migrations are generated/run with the **Api** as startup project:

```powershell
dotnet ef migrations add <Name> --project UBB_SE_2026_Jobs.Library --startup-project UBB_SE_2026_Jobs.Api
```

The Api calls `database.Database.Migrate()` at startup (`Program.cs`), so a running Api keeps the schema current — you usually do not need to run `database update` manually.

## Architecture

### Shared-interface / proxy pattern (key concept)

Service interfaces (e.g. `IJobService`, `IChatService`, `ICompanyService`) are defined in the **Library** and have **two implementations**:

- The **real implementation** (e.g. `JobService`) lives in the Library and is registered in the **Api**'s `Program.cs` as `Scoped`, backed by repositories + `JobsDbContext`.
- A **service proxy** (e.g. `JobServiceProxy` in `Library/ServiceProxies`) implements the same interface by making HTTP calls to the Api. The **App** registers proxies in `App.xaml.cs` via `AddHttpClient<TService, TProxy>(...)`.

So a ViewModel depends on `IJobService` and gets a proxy at runtime; the same interface in the Api is the DB-backed service. When you add a feature that crosses the client/server boundary, you typically touch: the interface (Library), the real service + repository (Library, registered in Api), a controller (Api), and the proxy (Library, registered in App).

### Authentication

JWT bearer. The Api validates tokens (`Program.cs`). On the App side, `SessionContext` holds the current token and `JwtForwardingHandler` (an `HttpMessageHandler` added to every proxy's `HttpClient`) attaches it to outgoing requests.

### Tests & Interviews (TI) — base URL split

The App's `Configuration` distinguishes two backends in `appsettings.json`:
- `Api:BaseUrl` (default `https://localhost:7134`) — the main PussyCats/Jobs API.
- `Api:TiBaseUrl` (default `http://localhost:5179`) — used by TI-only services.

TI-only services (`ITiAuthService`, `ITiTestService`, `ITiApplicantService`, `ITiSlotsService`, etc., under `App/Services/TI`) point at `TiBaseUrl`. All TI endpoints (`api/tests/*`, `api/testattempts/*`, `api/questions/*`, `api/answers/*`, `api/leaderboard/*`) are served by `UBB_SE_2026_Jobs.Api`. The same codebase and database back both base URLs — the port difference is a local-dev deployment detail. The TI **job catalog and skills** are owned by the main API, so they use `IJobService`/`ISkillService` on `BaseUrl` — see the comment block in `App.xaml.cs` before choosing a base URL for a TI call.

### Test system — data model and submission pipeline

The test system is distinct from personality tests and SkillTests. Its entities all live in `Library/Domain/Core/`:

```
Test (seeded, persistent)
  └─ TestQuestion[]  (seeded, per-test)
       └─ Answer[]   (per attempt, one per question)
TestAttempt          (one row per user-per-take, replayable)
  ├─ ExternalUserId  → User.UserId
  ├─ TestId          → Test.Id
  ├─ Status          "IN_PROGRESS" | "COMPLETED"
  ├─ Score           raw decimal (sum of CORRECT:X answer values)
  ├─ PercentageScore (Score / maxPossible) * 100, set by DataProcessingService
  ├─ IsValidated     true when DataProcessingService accepts the attempt
  └─ Answers[]       → Answer entities for this attempt
```

**Seeded tests** (8 total, `TestConfiguration.cs` + `QuestionConfiguration.cs`):

| ID | Title | Category | Questions | Max score |
|----|-------|----------|-----------|-----------|
| 1 | C# Fundamentals | Programming | 3 | 30 |
| 2 | SQL Basics | Databases | 3 | 30 |
| 3 | JavaScript Essentials | Web Development | 3 | 30 |
| 4 | Python Fundamentals | Programming | 3 | 30 |
| 5 | Java Fundamentals | Programming | 3 | 30 |
| 6 | DevOps Basics | Operations | 3 | 30 |
| 7 | Data Science Basics | Data Science | 3 | 30 |
| 8 | UI/UX Fundamentals | Design | 3 | 30 |

All questions are `SINGLE_CHOICE`, 10 pts each. `QuestionAnswer` is the **zero-based index** of the correct option in `OptionsJson`.

**Replayability:** Tests can be retaken unlimited times. Each take creates a new `TestAttempt`. `FindByUserAndTestAsync` returns only the most recent **IN_PROGRESS** attempt (not any completed one), so the submission flow always targets the active take. `AttemptValidationService.CheckExistingAttemptsAsync` only blocks if an IN_PROGRESS attempt already exists — not if prior completed attempts exist.

**Desktop submission flow** (`TiTestService` → `POST api/tests/submit-attempt`):
1. `TestsController.SubmitAttempt` calls `TestService.SubmitAttemptAsync(userId, testId, answers)`.
2. `SubmitAttemptAsync` finds the IN_PROGRESS attempt via `FindByUserAndTestAsync`, saves raw `Answer` rows, then calls `SubmitTestAsync(attempt.Id)`.
3. `SubmitTestAsync` loads answers via `AnswerRepository.FindByAttemptAsync` (includes `Question` via `ThenInclude`), grades each answer with `GradingService`, calls `CalculateFinalScore(attempt)` which iterates `attempt.Answers` and sums `CORRECT:X`/`PARTIAL:X` values into `attempt.Score`, then calls `attempt.Submit()` (sets `Status = "COMPLETED"`, `CompletedAt = now`), then persists via `UpdateAsync`.
4. `DataProcessingService.ProcessFinalizedAttemptAsync` re-loads the attempt (with answers + questions), validates it, sets `IsValidated = true`, and computes `PercentageScore = (attempt.Score / maxPossibleScore) * 100` where `maxPossibleScore = attempt.Answers.Sum(a => a.Question.QuestionScore)`.
5. `TestsController.SubmitAttempt` then calls `LeaderboardService.RecalculateAsync(testId)`.
6. The final `Score` is returned to the caller via `FindByIdAsync(attempt.Id)`.

**Web submission flow** (`TestsController.Submit` in the MVC project):
The web does grading client-side through separate API calls (`GradeAnswerAsync` per answer, `CalculateFinalScoreAsync` for the total), then sets `Status = "COMPLETED"`, `Score`, `PercentageScore`, and `IsValidated = true` on the DTO directly, persists via `PUT api/testattempts/{id}`, and then calls `LeaderboardApiClient.RecalculateLeaderboardAsync(testId)`. `ProcessFinalizedAttemptAsync` is **not** called in the web path — the web computes everything itself.

**`GradingService`** (`Library/Services/GradingService.cs`):
- `GradeSingleChoice`: compares `answer.Value.Trim()` to `question.QuestionAnswer.Trim()` (both are 0-based index strings). On match, sets `answer.Value = "CORRECT:{score}"`. On mismatch, leaves value unchanged.
- `CalculateFinalScore(attempt)`: iterates `attempt.Answers` (the navigation collection — must be loaded), sums `CORRECT:X` and `PARTIAL:X` values into `attempt.Score`. **Requires `attempt.Answers` to be populated**; if the collection is empty, score = 0.

**`DataProcessingService`** (`Library/Services/DataProcessingService.cs`):
- `ProcessFinalizedAttemptAsync(attemptId)`: validates user/test existence, `Status == "COMPLETED"`, `Score >= 0`, then sets `IsValidated = true` and computes correct `PercentageScore`. No time-based validity gate — the previous 3-month window was removed.
- The `attempt` is loaded via `FindByIdAsync` which includes `Answers → Question` (needed to compute `maxPossibleScore`).

**`LeaderboardService.RecalculateAsync(testId)`** (`Library/Services/LeaderboardService.cs`):
- Calls `FindValidAttemptsByTestIdAsync(testId)` → `Status == "COMPLETED" && IsValidated && PercentageScore != null && CompletedAt != null`, grouped by user (best `PercentageScore` per user), ordered descending.
- Deletes existing `LeaderboardEntry` rows for the test.
- Re-inserts ranked entries with `NormalizedScore = attempt.PercentageScore`.

**`TestAttemptRepository.UpdateAsync` — EF Core gotcha:**
`UpdateAsync` only calls `SaveChangesAsync()`. For **tracked** entities (loaded in the same DbContext scope, as in the server-side `SubmitTestAsync` path), this is sufficient. For **untracked/detached** entities (created from DTOs, as in `TestAttemptsController.Update` or the web's `PUT api/testattempts/{id}` path), `UpdateAsync` now calls `context.TestAttempts.Update(entity)` first to attach and mark as modified before saving. Without this, DTO-sourced updates silently no-op.

**Test Attempts tab** (desktop: `TestDashboardPage`, web: `SkillTestsController/Index`):
- Desktop: `TestDashboardViewModel` calls `ITiTestService.GetAttemptsByUserAsync(userId)` → `GET api/testattempts/byuser/{userId}` → `TestAttemptService.FindByUserId` → `FindCompletedByUserIdAsync` (filters `Status == "COMPLETED" && CompletedAt != null`). Shows one card per attempt; multiple attempts for the same test each get their own card.
- Web: `SkillTestsController.Index` calls `ISkillTestService.GetTestsForUserAsync(userId)` → also hits `FindCompletedByUserIdAsync` in-process.
- Score shown on cards: `ViewModelSupport.TiPercentage(attempt.Score, maxPossibleScore)` = `rawScore / maxPossibleScore * 100`, computed client-side from the raw `Score` field (not from `PercentageScore`).

### Web authentication and service registration

The web uses **two auth layers that work together**:

1. **Cookie auth** (browser ↔ Web) — `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)`. The browser holds a cookie; the web MVC app reads it as `ClaimsPrincipal`. Claims set at login: `ClaimTypes.NameIdentifier` (userId), `ClaimTypes.Role` ("Candidate"/"Recruiter"/"Admin"), `"CompanyId"` (recruiters only).

2. **JWT bearer** (Web → Api) — When `AccountController` logs in, it calls the Api which returns a JWT. The web stores that JWT in the ASP.NET session under `SessionKeys.JwtToken`. Every outbound `HttpClient` call to the Api goes through `JwtForwardingHandler` (`Web/Infrastructure/JwtForwardingHandler.cs`), which reads the token from the session and attaches it as `Authorization: Bearer …`. So the Api sees a valid JWT even though the browser only sees a cookie.

`JwtSessionFilter` (`Web/Infrastructure/JwtSessionFilter.cs`) guards against cookie/session drift: if the cookie says authenticated but the session has no JWT (e.g. server restart), it signs the user out and redirects to login rather than letting the request reach the Api with no token.

**Web service registration pattern** (Web's `Program.cs`):

The web uses the same `RegisterServiceProxy<TService, TProxy>` helper as the desktop for Library-interface services (e.g. `IChatService`, `IJobService`). All service proxies and TI API clients go through `JwtForwardingHandler`. For TI-specific raw API clients (concrete classes like `TestsApiClient`, `LeaderboardApiClient`) a separate helper `RegisterApiClient<TClient>` is used — these are typed `HttpClient` wrappers with no interface, injected directly into controllers.

The web has **only one base URL** (`ApiConfiguration.BaseUrl`). There is no `TiBaseUrl` on the web side — all TI endpoints hit the same host as the main API.

**Web session keys** (`Web/Infrastructure/SessionKeys.cs`):
- `SessionKeys.JwtToken` — the JWT string stored after login
- `SessionKeys.Mode` — `"Company"` or `"Candidate"`, determines which views/nav items are active

---

### Chat system

`IChatService` (`Library/Services/ChatService/IChatService.cs`) is shared by both clients. The desktop registers `ChatServiceProxy` (Library); the web also registers `ChatServiceProxy` via `RegisterServiceProxy<IChatService, ChatServiceProxy>`. The server-side `ChatService` runs in the Api.

**Two chat types:**

| Type | Created by | Participants |
|------|-----------|-------------|
| User–User | `FindOrCreateUserChatAsync` | Two users; must be same role AND (for recruiters) same company |
| User–Company | `FindOrCreateUserCompanyChatAsync` | One candidate + one company (optionally linked to a `Job`) |

**Key business rules in `ChatService`:**
- Recruiters can only chat with recruiters **from the same company** (enforced in `FindOrCreateUserChatAsync` via `IRecruiterRepository.GetCompanyIdForUserAsync`).
- Candidates can only chat with other candidates (same method enforces role parity).
- Block/unblock: only the blocker can call `UnblockChatAsync`; a blocked chat is hidden from the other party's list entirely.
- Soft delete: sets `DeletedAtByUser` or `DeletedAtBySecondParty` timestamp. Messages sent before the deletion timestamp are filtered from that user's view; the chat can be "undeleted" by creating it again via `FindOrCreate*`.

**File attachments:**
- Desktop: file picker → `ILocalFileStorageService.SaveFileAsync` → sends stored path via `SendStoredAttachmentAsync`
- Web: `IFormFile` upload → `ILocalFileStorageService.SaveFileAsync` → same `SendStoredAttachmentAsync`
- Files are served by `Api/Controllers/FilesController.cs` at `GET api/files/{filename}`
- Allowed: images (.jpg/.jpeg/.png ≤ 10 MB), files (.pdf/.docx/.doc ≤ 20 MB)

**Web `ChatController` specifics:**
- `IsCompanyMode()` reads `SessionKeys.Mode` from session — determines whether to call `GetChatsForUserAsync` vs `GetChatsForCompanyAsync`, and whether search returns users or recruiters-by-company.
- `ViewBag.ApiBase` is set to `apiConfiguration.BaseUrl + "/api/files"` so the view can build direct download URLs for attachments.
- `SearchUsers` endpoint returns different results based on mode: company mode → recruiters in same company; user mode → non-recruiter users.

**Desktop `ChatViewModel` specifics:**
- Two tabs: "Users" and "Companies". Companies tab searches `SearchCompaniesAsync` and starts a user-company chat; Users tab searches `SearchUsersAsync` and starts a user-user chat.
- `SelectedChat` setter triggers `LoadSelectedChatAsync` which loads messages and updates block/delete button visibility.
- Attachment send: opens file dialog, saves locally, then calls `SendStoredAttachmentAsync`.

**`ChatServiceProxy` (Library)** — used by both clients. Key methods map to `api/chats/*` endpoints. The `companyId` query param is optional and used when a recruiter is the caller (company-side participant) rather than a regular user.

---

### App DI conventions

`App.xaml.cs` → `ConfigureServices`:
- All proxies registered through the `RegisterServiceProxy<TService, TProxy>` helper (sets base URL + JWT handler).
- ViewModels are auto-registered by reflection: every non-abstract class whose name ends in `ViewModel` is added as `Transient` (`RegisterViewModels`). New ViewModels need no manual registration.
- Resolve services anywhere via `App.Services.GetRequiredService<T>()`.

### Database

SQL Server LocalDB. Connection string `JobsDb` in `UBB_SE_2026_Jobs.Api/appsettings.json` (`Server=(localdb)\mssqllocaldb;Database=Jobs_ISS_Final;...`).

## Repo state gotchas

- **`PussyCats` is still a deliberate name in places.** The old *PussyCats* app was de-merged into this solution; all namespaces are now `UBB_SE_2026_Jobs.*`. But some **server-side type names** intentionally keep the prefix — `PussyCatsJobService`/`IPussyCatsJobService`, `PussyCatsCompanyService`/`IPussyCatsCompanyService`, `PussyCatsCompanyRepository`, `JobsDbContext`'s historical notes — and many `// Original: PussyCatsApp …` provenance comments mark ported logic. These are not stragglers; don't blanket-rename `PussyCats`. The client-facing interfaces the App/proxies use are the un-prefixed ones (`IJobService`, `ICompanyService`), which coexist with the prefixed server interfaces.
- **Personality questions:** the domain type is `PersonalityQuestion` (EF-mapped, `[Key]`); an older alias `Question` was removed during the de-merge. Use `PersonalityQuestion`.
- App secrets: copy `UBB_SE_2026_Jobs.App/appsettings.local.json.example` to `appsettings.local.json` to override `Api:BaseUrl` locally (gitignored, copied to output on build).
