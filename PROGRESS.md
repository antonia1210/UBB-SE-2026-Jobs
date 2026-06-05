# PROGRESS.md — Refactor Tracking

**Last updated:** 2026-06-05  
**Branch:** `chat-rework`

---

## Current state

The solution is a merger of two formerly separate projects — **PussyCats** (job matching, personality tests, chat, match pipeline) and **TI/Jobs** (tests & interviews, scheduling, leaderboard). The merger left several structural problems: hardcoded placeholder values, stub controller actions, and diverging data paths between the web and desktop clients.

This document tracks what has been completed and what remains outstanding for an agent resuming this work.

---

## Completed

### 1. `TemporaryCompanyId` removed (critical bug fix)

**Problem:** All web recruiter views (Applicants, Ranked Applicants, Applicant Decision, Chat) used a hardcoded company ID of `1` (database name: TechNova), so every recruiter regardless of their actual company always saw TechNova's data.

**Root cause:** `ApiConfiguration.TemporaryCompanyId = 1` was a placeholder left from before auth was wired up. The `CompanyId` claim was already being set in the cookie during login (`AccountController.cs` line 159-162) but was never read by the controllers.

**Fix applied:**
- `Web/Configuration/ApiConfiguration.cs` — removed `TemporaryCompanyId` property entirely
- `Web/appsettings.json` — removed `"TemporaryCompanyId": 1` from `Api` section
- `Web/Controllers/CompanyRecommendationsController.cs` — removed `ApiConfiguration` injection; added `GetCompanyId()` that reads `User.FindFirstValue("CompanyId")`
- `Web/Controllers/CompanyStatusController.cs` — same; also removed stub `Create`/`Delete` actions (see §2)
- `Web/Controllers/MatchesController.cs` — removed `ApiConfiguration` injection; added `GetCompanyId()`
- `Web/Controllers/ChatController.cs` — removed `TemporaryCompanyId` fallback from `GetCompanyId()`; now throws if claim is missing
- `Web/Views/Shared/_Layout.cshtml` — removed `@inject ApiConfiguration`; replaced `@ApiConfiguration.TemporaryCompanyId` in Company Profile nav link with `@User.FindFirst("CompanyId")?.Value`

**All four routes now correctly scope to the logged-in recruiter's actual company.**

---

### 2. Stub `Create`/`Delete` actions removed from `CompanyStatusController`

**Problem:** `CompanyStatusController` had four no-op actions — `Create()` GET, `Create(model)` POST, `Delete(id)` GET, and `DeleteConfirmed(id)` POST. The GET actions showed views with disabled buttons and text like "not supported" / "entries are created automatically from matches". The POST actions did nothing (just redirected to Index). None were linked from the UI.

**Fix applied:**
- Removed all four stub actions from `Web/Controllers/CompanyStatusController.cs`
- Deleted `Web/Views/CompanyStatus/Create.cshtml` (disabled form with info alert)
- Deleted `Web/Views/CompanyStatus/Delete.cshtml` (disabled form with warning alert)
- `Details.cshtml`, `Edit.cshtml`, and `Index.cshtml` remain intact

---

### 3. Hardcoded JWT secret moved to configuration (`TestsAuthService`)

**Problem:** `Library/Services/TestsAuthService.cs` had `private const string SecretKey = "O_CHEIE_SECRET_FOARTE_LUNGA_SI_SIGURA_AICI_12345!"` — a plaintext hardcoded JWT signing key for the TI authentication system. This is a security vulnerability; the key must be in configuration so it can be rotated and kept out of source control.

**Fix applied:**
- `Library/Services/TestsAuthService.cs` — injected `IConfiguration`; reads key from `configuration["TiJwt:Key"]`; throws `InvalidOperationException` at startup if missing (fail-fast)
- `Api/appsettings.json` — added `"TiJwt": { "Key": "TiSecretKeyForJwtAuthenticationMustBeLongEnough12345!" }` section

**Note:** The actual production secret key in `TiJwt:Key` should be replaced with a strong random value and stored in a secrets manager (Azure Key Vault, user secrets, environment variables) — not committed to source. The placeholder value currently in appsettings.json is for local development only.

---

### 4. Test system overhaul — replayability, leaderboard, tab rename, bug fixes (2026-06-05)

This is a comprehensive overhaul of the test-taking and leaderboard pipeline. All changes were made on branch `chat-rework`.

#### 4a. "Skill Tests" → "Test Attempts" rename

The tab was renamed across all five touch-points (no logic changes, cosmetic only):

| File | Change |
|------|--------|
| `App/MainWindow.xaml` | Nav item `Content="Skill Tests"` → `Content="Test Attempts"` |
| `App/Views/Candidate/TestDashboardPage.xaml` | Hero `Text="Skill Tests"` → `Text="Test Attempts"`, subtitle updated |
| `App/Views/Candidate/UserProfilePage.xaml` | Button `Content="Skill Tests"` → `Content="Test Attempts"` |
| `Web/Views/Shared/_Layout.cshtml` | Sidebar label `Skill Tests` → `Test Attempts` |
| `Web/Views/SkillTests/Index.cshtml` | Page title, h1, subtitle, badge caption all updated |

#### 4b. Replayability — tests can now be retaken

Previously the system blocked any second attempt. Every completed test now returns the user to a fresh attempt instead.

**`Library/Repositories/TestAttemptRepository.cs` — `FindByUserAndTestAsync`:**  
Changed to return only the most recent **IN_PROGRESS** attempt (was: any attempt). The submission flow calls this to locate the active attempt; completed attempts are irrelevant to it.

**`Library/Services/AttemptValidationService.cs` — `CheckExistingAttemptsAsync` / `CanStartTestAsync`:**  
Both methods now only block if an IN_PROGRESS attempt already exists (was: blocked on any prior attempt). Since `FindByUserAndTestAsync` returns IN_PROGRESS only, a completed attempt no longer prevents a new start.

**`Web/Controllers/TestsController.cs` — `Take` action:**  
No longer shows "AlreadyTaken" view. Checks for an IN_PROGRESS attempt via `GetAttemptByUserAndTestAsync`; if none, starts a fresh one.

**`Web/Views/Tests/Index.cshtml`:**  
Info banner updated from "Tests can only be taken once" to "Tests are replayable". "Already Completed" disabled button replaced with "Retake Test" active link.

**`App/ViewModels/TI/TiTestPageViewModel.cs`:**  
Removed `AlreadyAttempted` property. `LoadAsync` now resumes an IN_PROGRESS attempt or starts a new one; never blocks.

**`App/Views/TestsAndInterviews/TiTestPage.xaml.cs`:**  
Removed the `AlreadyAttempted` navigation redirect after `LoadAsync`.

**`App/Views/TestsAndInterviews/TiMainTestPage.xaml`:**  
Info banner updated to say tests are replayable and that the highest score counts on the leaderboard.

#### 4c. Multi-attempt support in the Test Attempts tab

The tab previously crashed when a user had more than one attempt for the same test (due to `ToDictionary` throwing on duplicate keys).

**`App/ViewModels/TestDashboardViewModel.cs`:**  
Replaced `ToDictionary(a => a.TestId)` with a per-attempt loop using test and question-count caches. Now shows one card per attempt — full history, all takes visible.

**`Web/Clients/TestsApiClient.cs`:**  
Added `GetAttemptsByUserAsync(int userId)` → `GET api/testattempts/byuser/{userId}`.

**`Web/Controllers/TestsController.cs` — `Index` action:**  
Replaced N+1 per-test attempt lookups with a single bulk `GetAttemptsByUserAsync` call. Uses a `HashSet<int>` of completed test IDs to mark which tests have been taken.

#### 4d. Leaderboard deduplication

**`Library/Repositories/TestAttemptRepository.cs` — `FindValidAttemptsByTestIdAsync`:**  
Now groups by `ExternalUserId` and takes only the highest-`PercentageScore` attempt per user (earliest `CompletedAt` on tie) before ranking. Previously all attempts appeared separately, so a user with three low scores outranked a user with one high score.

#### 4e. Expanded seed data

| ID | Title | Category |
|----|-------|----------|
| 4 | Python Fundamentals | Programming |
| 5 | Java Fundamentals | Programming |
| 6 | DevOps Basics | Operations |
| 7 | Data Science Basics | Data Science |
| 8 | UI/UX Fundamentals | Design |

- `Library/Persistence/Configurations/TestConfiguration.cs` — added tests 4–8
- `Library/Persistence/Configurations/QuestionConfiguration.cs` — added questions 10–24 (3 per new test, all `SINGLE_CHOICE`, 10 pts each, zero-based `QuestionAnswer` indices)
- EF migration `SeedAdditionalTests` generated and applied automatically on API startup

#### 4f. Bug fix — score = 0 on desktop

**Root cause:** `TestService.SubmitAttemptAsync` called `FindByUserAndTestAsync(userId, testId)` _after_ `SubmitTestAsync` had already marked the attempt `COMPLETED`. `FindByUserAndTestAsync` now only returns IN_PROGRESS attempts → returned null → always returned `DefaultSubmissionScore = 0f`.

**Fix (`Library/Services/TestService.cs`):**  
The final score fetch now uses `FindByIdAsync(attempt.Id)` which returns the attempt regardless of status.

#### 4g. Bug fix — leaderboard always empty

Three compounding root causes:

**1. 3-month validity window (`Library/Services/DataProcessingService.cs`):**  
`IsTestStillValidForLeaderboard` checked `test.CreatedAt.AddMonths(3) >= DateTime.UtcNow`. All seeded tests have `CreatedAt = 2026-01-01`; the window expired 2026-04-01. Every attempt was rejected with `IsValidated = false`. Fix: removed `IsTestStillValidForLeaderboard` entirely. No time-based gate.

**2. `ConvertToPercentageScore` was an identity function (`Library/Services/DataProcessingService.cs`):**  
The formula was `originalScore / 100m * 100m = originalScore`. For a test with 3 correct answers × 10 pts = 30 raw score, `PercentageScore` was stored as 30, not 100. Fix: `PercentageScore` is now computed as `(attempt.Score / maxPossibleScore) * 100` where `maxPossibleScore = attempt.Answers.Sum(a => a.Question.QuestionScore)` (loaded via `FindByIdAsync`'s `ThenInclude`).

**3. Raw score cap of 100m (`Library/Services/DataProcessingService.cs`):**  
`ValidateAttemptAsync` rejected attempts with `Score > 100m`. With 3 questions × 10 pts = 30 raw this passed, but any test with more questions would be silently rejected. Fix: removed the upper cap, keeping only `Score < 0` as invalid.

**4. Desktop never triggered recalculation (`Api/Controllers/TestsController.cs`):**  
The `POST api/tests/submit-attempt` endpoint returned the score but never called `LeaderboardService.RecalculateAsync`. `LeaderboardEntry` rows were only created when the web's `RecalculateLeaderboardAsync` was called. Fix: `TestsController.SubmitAttempt` now injects `ILeaderboardService` and calls `RecalculateAsync(dto.TestId)` after every desktop submission.

#### 4h. Bug fix — Test Attempts tab empty on web

**Root cause:** The web's `TestsController.Submit` updates the attempt by: building a DTO from the API response, setting `Status = "COMPLETED"` etc. on it, then calling `PUT api/testattempts/{id}`. `TestAttemptsController.Update` creates a new `TestAttempt` entity via `dto.ToEntity()` — this entity is **detached** (never tracked by EF Core). `TestAttemptRepository.UpdateAsync` called only `SaveChangesAsync()` without attaching the entity, so the call silently no-opped and the attempt remained `IN_PROGRESS` in the database forever. `FindCompletedByUserIdAsync` filters `Status == "COMPLETED"` → returned nothing → tab was empty.

**Fix 1 (`Library/Repositories/TestAttemptRepository.cs`):**  
`UpdateAsync` now checks `databaseContext.Entry(testAttempt).State == EntityState.Detached`. If detached, it calls `databaseContext.TestAttempts.Update(testAttempt)` before `SaveChangesAsync()`. For tracked entities (the server-side submission path), no change in behavior.

**Fix 2 (`Web/Controllers/TestsController.cs`):**  
`Submit` now sets `attempt.IsValidated = true` on the DTO before persisting. The web submission path bypasses `DataProcessingService.ProcessFinalizedAttemptAsync` (which is the only other place `IsValidated` is set to `true`), so without this, web-submitted attempts would never qualify for the leaderboard (`FindValidAttemptsByTestIdAsync` requires `IsValidated = true`).

---

### 5. Chat system rework (2026-06-05)

Rewrote the chat subsystem across both clients and the server to fix correctness issues, add missing features, and enforce business rules. All changes on branch `chat-rework`.

#### What changed

**`Library/Services/ChatService/ChatService.cs` + `IChatService`:**
- Added `SearchRecruitersByCompanyAsync(int companyId, string query)` — lets a logged-in recruiter search for colleagues at the same company to start a peer chat. Previously, recruiter-to-recruiter chat was impossible.
- `FindOrCreateUserChatAsync` now validates role parity and company match before creating a chat (throws `InvalidOperationException` if a candidate tries to chat with a recruiter, or a recruiter tries to chat with someone from a different company). Previously no such guard existed.
- Soft-delete: `DeleteChatAsync` now sets `DeletedAtByUser`/`DeletedAtBySecondParty` timestamps instead of hard-deleting. `GetMessagesAsync` filters messages before the deletion timestamp so the other participant's history is unaffected.
- Block/unblock enforcement: `UnblockChatAsync` now verifies `chat.BlockedByUserId == unblockerId` — only the blocker can unblock.
- `IRecruiterRepository.GetCompanyIdForUserAsync` and `GetUserIdsByCompanyAsync` were added to `IRecruiterRepository`/`RecruiterRepository` to support the company-membership checks in `FindOrCreateUserChatAsync` and `SearchRecruitersByCompanyAsync`.

**`Api/Controllers/ChatsController.cs`:**
- Added `DELETE api/chats/{id}` (soft delete), `PATCH api/chats/{id}/block`, `PATCH api/chats/{id}/unblock`.
- `GET api/chats/search/recruiters?companyId=&query=` → `SearchRecruitersByCompanyAsync`.
- `POST api/chats/attachments` → `SendStoredAttachmentAsync` (separate from text-message path).

**`Library/ServiceProxies/ChatServiceProxy.cs`:**
- Added proxy methods for all new endpoints: `BlockChatAsync`, `UnblockChatAsync`, `DeleteChatAsync`, `SendStoredAttachmentAsync`, `SearchRecruitersByCompanyAsync`.

**Desktop `App/ViewModels/ChatViewModel.cs`:**
- Rewrote from scratch (old version had a polling loop and direct HTTP calls). Now uses `IChatService` exclusively.
- Two tabs: "Users" (candidate-to-candidate) and "Companies" (candidate-to-company/job). Tab state is synced via `IsSyncingTabState` guard to prevent recursive notification loops.
- Block/Unblock/Delete commands wired to `SelectedChat`; button visibility driven by `showBlock`/`showUnblock`/`showGoToProfile` etc.
- Attachment send: file picker → `ILocalFileStorageService.SaveFileAsync` → `SendStoredAttachmentAsync`.

**`App/Views/ChatPage.xaml` + `ChatPage.xaml.cs`:**
- Simplified code-behind (removed old polling and direct dependency on `ChatsController`).
- XAML updated to bind to the new `ChatViewModel` properties and commands.

**Web `Web/Controllers/ChatController.cs`:**
- Replaced ad-hoc API calls with direct `IChatService` injection (same service proxy pattern used for all other web services).
- `IsCompanyMode()` reads `SessionKeys.Mode` from session — determines recruiter vs. candidate behavior.
- `SearchUsers` returns recruiters-by-company in company mode, non-recruiter users in user mode.
- `Send`, `SendAttachment`, `Block`, `Unblock`, `Delete`, `StartChat` actions all implemented.

**`Web/Views/Chat/Index.cshtml` + `Show.cshtml`:**
- Index: two-tab layout (Users / Companies), live search with debounce, chat list with last-message preview.
- Show: message thread with attachment rendering, block/unblock/delete buttons, file download links pointing to `ViewBag.ApiBase`.

#### Key invariants to preserve
- `FindOrCreateUserChatAsync` must keep the role-parity and same-company checks — removing them would allow candidates to message recruiters directly.
- `SendStoredAttachmentAsync` path (web) and `StoreAttachmentAsync` path (desktop upload) are separate. The desktop picks a local file, saves it, then sends the stored path. The web receives an `IFormFile`, saves it server-side, then calls `SendStoredAttachmentAsync`. Don't conflate the two.
- `ShouldIncludeChat` filters out chats blocked by the *other* party (not the caller) — a blocked caller still sees the chat in their list but cannot send.

---

### 6. Recruiter test access and manual test authoring removed (2026-06-05)

Tests are now treated as static seeded content for candidates to replay. Recruiters should not reach the test catalog or any test/question authoring UI.

**Fix applied:**
- `App/MainWindow.xaml.cs` - moved `TiMainTestPage` from shared navigation to candidate-only navigation, hiding the desktop Tests item from recruiter mode.
- `Web/Views/Shared/_Layout.cshtml` - hid the MVC Tests navigation link unless the user is a candidate.
- `Web/Controllers/TestsController.cs` - restricted `Index` and `Details` to candidates and removed recruiter/admin `Create`, `Edit`, and `Delete` actions.
- `Web/Views/Tests/Index.cshtml` - removed the "Create New Test", "Edit", and "Delete" controls.
- Deleted obsolete web test authoring views: `Web/Views/Tests/Create.cshtml`, `Edit.cshtml`, and `Delete.cshtml`.
- `Api/Controllers/TestsController.cs`, `ITestService`, `TestService`, `ITestRepository`, and `TestRepository` - removed test create/update/delete endpoints and service/repository methods.
- Removed the old MVC question-management surface (`Web/Controllers/QuestionsController.cs` and `Web/Views/Questions/*`) and removed question create/update/delete endpoints from `Api/Controllers/QuestionsController.cs`. Read-only question endpoints remain for candidate test-taking.

**Build verification:**
- `dotnet build UBB_SE_2026_Jobs.Api\UBB_SE_2026_Jobs.Api.csproj --no-restore -p:UseSharedCompilation=false -v minimal` - succeeded with existing warnings.
- `dotnet build UBB_SE_2026_Jobs.Web\UBB_SE_2026_Jobs.Web.csproj --no-restore -p:UseSharedCompilation=false -v minimal` - succeeded with existing warnings.
- `dotnet build UBB_SE_2026_Jobs.App\UBB_SE_2026_Jobs.App.csproj --no-restore -p:Platform=x64 -p:UseSharedCompilation=false -v minimal` - succeeded with existing warnings.

---

### 7. Latest main merge and Visual Studio F5 SQL config (2026-06-05)

Pulled the latest `origin/main` into the local branch without committing or pushing.

**Conflict resolution:**
- `Web/Controllers/TestsController.cs` - kept candidate replayability state (`HasBeenTaken`) while preserving candidate-only access to the test catalog.
- `Web/Views/Tests/Index.cshtml` - kept the Start/Retake behavior and removed recruiter/admin Edit/Delete controls.

**F5/local SQL configuration:**
- `Api/appsettings.json` - changed `JobsDb` from `LucaT2\MSSQLSERVER01` to `DESKTOP-G09FIFT`.
- `Api/appsettings.Development.json` - added the same `DESKTOP-G09FIFT` override so Visual Studio F5 does not fall back to LocalDB or another machine.
- `Library/Helpers/Env.cs` - changed the shared connection string to `DESKTOP-G09FIFT`.
- `Api/Program.cs` - uses console/debug logging explicitly to avoid Windows Event Log write failures during local startup.

**Verification:**
- `dotnet build UBB_SE_2026_Jobs.Api\UBB_SE_2026_Jobs.Api.csproj --no-restore -p:UseSharedCompilation=false -v minimal` - succeeded with existing warnings.
- API startup reached SQL connection/migration, but this Codex shell still hit `Cannot generate SSPI context` for Windows auth. Visual Studio may behave differently under the interactive user session; the code/config side now uses the requested machine name.

---

## Outstanding tasks

### HIGH PRIORITY

#### A. Job application data parity (web vs. desktop)

**Problem:** When a candidate applies for a job, the web and desktop take completely different code paths:

| Client | Action | API endpoint | Creates |
|--------|--------|-------------|---------|
| **Web** | Like button on JobBrowser | `POST /api/applicants` then match creation | `Applicants` row + `Matches` row |
| **Desktop** | Like button on UserRecommendationPage | `POST /api/recommendations/{userId}/like` | `Matches` row only |

Result: desktop-applied candidates never appear in the `Applicants` table that the TI system reads from. `AppTestGrade`, `CvGrade`, `CompanyTestGrade`, `InterviewGrade` on `Applicant` will always be null for desktop applicants.

**Fix needed:**
- Option A: Wire the desktop Like button to also create an `Applicant` row (call `POST /api/applicants` after the recommendation like), **OR**
- Option B: Change the TI system to source applicant data from `Matches` instead of `Applicants` (which requires schema work).

Option A is the lower-risk path since it doesn't require schema changes.

**Files to change for Option A:**
- `Library/ServiceProxies/UserRecommendationServiceProxy.cs` — after the like call, call the applicants endpoint
- Or create a new `IApplicantService` method that creates an `Applicant` row and call it from `UserRecommendationService.LikeJobAsync()`
- `Api/Controllers/RecommendationsController.cs` or `UserRecommendationsController.cs` — wire creation of Applicant row

---

#### B. Applicant grade columns have no write UI

**Problem:** `Applicant.AppTestGrade`, `Applicant.CvGrade`, `Applicant.CompanyTestGrade`, `Applicant.InterviewGrade` are displayed in:
- `Web/Views/Applicants/ApplicantsByJob.cshtml` (read-only table)
- `App/Views/TestsAndInterviews/TiJobApplicantsPage.xaml` (read-only list; also missing `CompanyTestGrade`)

But **no UI exists anywhere to write these values**. The only write path is `PUT /api/applicants/{id}` with no frontend calling it.

**Fix needed:** Either build a grade-entry form for recruiters (in web and/or desktop), or auto-populate these columns from `TestAttempt.Score` and `InterviewSession.Score` via a background job or post-evaluation hook. The `TestAttempt` and `InterviewSession` records are linked by `ExternalUserId` (= `User.UserId`), not by `ApplicantId`, so a join is needed.

**Files to read:** `Library/Domain/Applicant.cs`, `Library/Domain/TestAttempt.cs`, `Library/Domain/InterviewSession.cs`, `Library/Services/GradingService.cs`

---

#### C. Desktop `TiJobApplicantsPage` missing `CompanyTestGrade` column

**Problem:** `App/Views/TestsAndInterviews/TiJobApplicantsPage.xaml` shows only 3 of the 4 grade columns (App Grade, CV Grade, Interview) — `CompanyTestGrade` is absent from both the XAML binding and the ViewModel.

**Fix needed:** Add `CompanyTestGrade` column to `TiJobApplicantsPage.xaml` and ensure `TiApplicantsViewModel` exposes it.

---

#### D. `CompanyStatusPage` not accessible from navigation

**Problem:** `App/Views/Company/CompanyStatusPage.xaml` (the actual recruiter evaluation UI shown in screenshots) exists but is not in the desktop's `CompanyPages` list or reachable from the sidebar navigation. Recruiters cannot reach it from normal navigation.

**Files to check:** `App/MainWindow.xaml.cs` (PageMap and CompanyPages list)

---

### MEDIUM PRIORITY

#### E. Schema duplication: `Match` vs `Applicant` tables

**Problem:** Both tables track user-job applications, each owned by a different subsystem. The `Match` table (PussyCats) owns the application pipeline (Status, FeedbackMessage, Timestamp). The `Applicant` table (TI) owns evaluation scores. The two are never explicitly linked by FK.

**Recommended approach:** Add a nullable `MatchId` FK column to `Applicant` so they can be joined, without merging the tables (which would require a major migration and service refactor). This is the minimum viable fix that doesn't break either subsystem.

**Files:** `Library/Domain/Applicant.cs`, `Library/Persistence/JobsDbContext.cs`, new EF migration

---

#### F. Schema duplication: `Slot` vs `InterviewSession`

**Problem:** Both tables represent interview scheduling. `Slot` is managed by the PussyCats recruiter, `InterviewSession` is created by the TI booking flow. No FK links them, so an interview booked via the TI UI is invisible to the main recruiter view and vice versa.

**Recommended approach:** Add `SlotId` FK to `InterviewSession` to link them when a candidate books a slot.

---

#### G. Other schema duplications (document for future sprint)

Full list of identified duplications (see original audit):
1. `PersonalityTestResult` vs `TestAttempt` — two result tracking models
2. `User` + `Recruiter` split — `Recruiter.CompanyName` is redundant (should be read from `Company.Name`)
3. `Company` table bloat — game/scenario data crammed into 13 flat columns; `BuddyName`, `Scen1Text`, etc.
4. `Chat` stale cache fields — `LastMessageSnippet`, `LastMessageTime`, `LastMessage`, `UnreadCount` never read by any view
5. `CompanyPosting` vs `Job` — thin unused duplicate
6. `UserSkill` + `SkillTest` — two skill verification systems

---

### LOW PRIORITY

#### H. Dead views/pages audit

The following web views exist but have no active navigation entry points (documented for removal in a future pass):
- `Web/Views/Recommendations/` — all 5 views (no nav link)
- `Web/Views/SkillTests/Create.cshtml`, `Delete.cshtml`, `Details.cshtml`, `Edit.cshtml`, `SkillTestCard.cshtml` — no corresponding controller actions

The following desktop pages are never navigated to from main navigation:
- `App/Views/Candidate/ExportCVPage.xaml` — not in PageMap or CompanyPages/CandidatePages
- `App/Views/Candidate/CompanyProfilePage.xaml` — never navigated to from any ViewModel

---

#### I. Security: JWT keys in appsettings.json

Both `Jwt:Key` (main API JWT) and `TiJwt:Key` (TI auth JWT) are currently stored as plaintext strings in `Api/appsettings.json`. For production:
- Use `dotnet user-secrets` in development
- Use environment variables or a secrets manager in CI/CD and production
- Rotate the keys to strong random values (not the placeholder strings currently checked in)

---

## Build status

As of 2026-06-05, `dotnet build UBB_SE_2026_Jobs.slnx` produces **0 errors** (pre-existing warnings only; none introduced by any of the above work).

---

## Key files reference

### Auth / company routing
| File | Role |
|------|------|
| `Web/Controllers/AccountController.cs:143-172` | Login/register flow; sets `CompanyId` cookie claim for recruiters |
| `Web/Controllers/CompanyRecommendationsController.cs` | Ranked Applicants — reads company from JWT claim |
| `Web/Controllers/CompanyStatusController.cs` | Applicant pipeline — reads company from JWT claim; stubs removed |
| `Web/Controllers/MatchesController.cs` | Applicants list — reads company from JWT claim |
| `Web/Controllers/ChatController.cs` | Chat — reads company from JWT claim (no fallback) |

### Test system — core library
| File | Role |
|------|------|
| `Library/Domain/Core/Test.cs` | Persistent test entity; `Id`, `Title`, `Category`, `CreatedAt`, `Questions` nav |
| `Library/Domain/Core/TestQuestion.cs` | Question entity; `QuestionAnswer` = zero-based index string; `QuestionScore` = float pts |
| `Library/Domain/Core/TestAttempt.cs` | Per-user-per-take; `Status`, `Score`, `PercentageScore`, `IsValidated`, `Answers` nav |
| `Library/Domain/Core/Answer.cs` | Per-question response; `Value` mutated in-place by grading ("CORRECT:10", "PARTIAL:5") |
| `Library/Persistence/Configurations/TestConfiguration.cs` | Seeds tests 1–8 |
| `Library/Persistence/Configurations/QuestionConfiguration.cs` | Seeds questions 1–24 (3 per test, 10 pts each) |
| `Library/Repositories/TestAttemptRepository.cs` | `FindByUserAndTestAsync` → IN_PROGRESS only; `FindCompletedByUserIdAsync` → all COMPLETED; `FindValidAttemptsByTestIdAsync` → best per user for leaderboard; `UpdateAsync` → attaches detached entities before save |
| `Library/Repositories/AnswerRepository.cs` | `FindByAttemptAsync` → includes `Question` via ThenInclude |
| `Library/Services/TestService.cs` | `SubmitAttemptAsync`: saves answers, calls `SubmitTestAsync`, `ProcessFinalizedAttemptAsync`, returns score via `FindByIdAsync(attempt.Id)` |
| `Library/Services/GradingService.cs` | `GradeSingleChoice`: sets `answer.Value = "CORRECT:{score}"` on match; `CalculateFinalScore`: sums CORRECT:/PARTIAL: from `attempt.Answers` into `attempt.Score` |
| `Library/Services/DataProcessingService.cs` | `ProcessFinalizedAttemptAsync`: validates attempt, sets `IsValidated = true`, computes `PercentageScore = (Score / maxPossibleScore) * 100`; no time gate |
| `Library/Services/AttemptValidationService.cs` | `CheckExistingAttemptsAsync`: only blocks if IN_PROGRESS attempt exists |
| `Library/Services/LeaderboardService.cs` | `RecalculateAsync`: deletes old entries, re-ranks validated attempts (best per user) |

### Test system — API controllers
| File | Role |
|------|------|
| `Api/Controllers/TestsController.cs` | `POST start` → start attempt; `POST submit-attempt` → grade + process + recalculate leaderboard |
| `Api/Controllers/TestAttemptsController.cs` | CRUD for attempts; `GET byuser/{userId}` → completed attempts for tab; `PUT {id}` → update (used by web submit path) |

### Test system — desktop
| File | Role |
|------|------|
| `App/Services/TI/TiTestService.cs` | `StartAttemptAsync` → `POST api/testattempts`; `SubmitAttemptAsync` → `POST api/tests/submit-attempt`; `GetAttemptsByUserAsync` → `GET api/testattempts/byuser/{userId}` |
| `App/ViewModels/TestDashboardViewModel.cs` | Loads all attempts, creates one card per attempt; uses test/question caches to avoid N+1 |
| `App/ViewModels/SkillTestCardViewModel.cs` | `IsTiAttemptCompleted`: Status contains "complete" OR CompletedAt not null; score shown via `ViewModelSupport.TiPercentage(attempt.Score, maxPossibleScore)` |
| `App/ViewModels/ViewModelSupport.cs` | `TiPercentage`: `rawScore / maxPossibleScore * 100` (correct %); `IsTiAttemptCompleted`: loose status check |
| `App/ViewModels/TI/TiTestPageViewModel.cs` | Resumes IN_PROGRESS or starts fresh; never blocks on prior completed attempts |

### Test system — web
| File | Role |
|------|------|
| `Web/Controllers/TestsController.cs` | `Take`: resumes or starts fresh; `Submit`: grades client-side, sets `IsValidated = true`, updates attempt, recalculates leaderboard |
| `Web/Controllers/SkillTestsController.cs` | Test Attempts tab — calls `ISkillTestService.GetTestsForUserAsync` → `FindCompletedByUserIdAsync` |
| `Web/Clients/TestsApiClient.cs` | `GetAttemptsByUserAsync(userId)`, `StartAttemptAsync`, `SubmitAttemptAsync`, `GradeAnswerAsync`, `CalculateFinalScoreAsync`, `UpdateAttemptAsync` |

### Chat system
| File | Role |
|------|------|
| `Library/Services/ChatService/IChatService.cs` | Shared interface — both clients and server implement/use it |
| `Library/Services/ChatService/ChatService.cs` | Server implementation; enforces role-parity, company-match, block/unblock, soft-delete |
| `Library/ServiceProxies/ChatServiceProxy.cs` | HTTP proxy used by both App and Web; maps to `api/chats/*` endpoints |
| `Api/Controllers/ChatsController.cs` | REST endpoints for chat CRUD, messaging, block/unblock, search |
| `App/ViewModels/ChatViewModel.cs` | Desktop MVVM; two tabs (Users/Companies), block/delete commands, attachment send |
| `App/Views/ChatPage.xaml` | Desktop chat UI |
| `Web/Controllers/ChatController.cs` | Web MVC; mode-aware (recruiter vs. candidate), file upload, search |
| `Web/Views/Chat/Index.cshtml` | Web chat list; two-tab layout, live search |
| `Web/Views/Chat/Show.cshtml` | Web message thread; block/unblock/delete, file download |
| `Library/Repositories/RecruiterRepository.cs` | `GetCompanyIdForUserAsync`, `GetUserIdsByCompanyAsync`, `GetAllRecruiterUserIdsAsync` — used by ChatService for company-membership checks |

### Web infrastructure
| File | Role |
|------|------|
| `Web/Program.cs` | Service registration: `RegisterServiceProxy` (interface proxies) + `RegisterApiClient` (TI typed clients); cookie auth + session |
| `Web/Infrastructure/JwtForwardingHandler.cs` | Reads JWT from session, attaches as Bearer header on every outbound API call |
| `Web/Infrastructure/JwtSessionFilter.cs` | Detects cookie/session drift on each request; signs out if session lost JWT |
| `Web/Infrastructure/SessionKeys.cs` | Constants: `JwtToken`, `Mode` |
| `Web/Infrastructure/ModeAuthorizeFilter.cs` | Enforces role-based page access based on `SessionKeys.Mode` |
| `Web/Configuration/ApiConfiguration.cs` | `BaseUrl` — single base URL for all web → API calls (no TiBaseUrl split on web) |

### Other
| File | Role |
|------|------|
| `Library/Services/TestsAuthService.cs` | TI auth — JWT key from `TiJwt:Key` config |
| `Api/appsettings.json` | `TiJwt:Key`, `Jwt:Key`, connection string `JobsDb` |
| `Library/Domain/Applicant.cs` | TI evaluation data (grades); no FK to Match |
| `Library/Domain/Match.cs` | Application pipeline (Status, Feedback); no FK to Applicant |
