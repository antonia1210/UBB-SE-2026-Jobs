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
- `Web/Views/Questions/` — all 7 views (admin-only, no nav link)
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

### 5. Tests — Replayability, Leaderboard Fix, Tab Rename, Expanded Seed Data (2026-06-05)

**"Skill Tests" → "Test Attempts" rename:**
- `App/MainWindow.xaml` — nav item label
- `App/Views/Candidate/TestDashboardPage.xaml` — page hero title and subtitle
- `App/Views/Candidate/UserProfilePage.xaml` — button label
- `Web/Views/Shared/_Layout.cshtml` — sidebar nav label
- `Web/Views/SkillTests/Index.cshtml` — page title, hero h1, subtitle, badge caption

**Replayability (tests now takeable multiple times):**
- `Library/Repositories/TestAttemptRepository.cs` — `FindByUserAndTestAsync` now returns the most recent **IN_PROGRESS** attempt only (not any prior completed one); submission flow always targets the active attempt
- `Library/Services/AttemptValidationService.cs` — `CheckExistingAttemptsAsync` and `CanStartTestAsync` now only block if an IN_PROGRESS attempt exists (previously blocked on any prior attempt)
- `Web/Controllers/TestsController.cs` — `Take` action no longer shows "AlreadyTaken" view; always resumes or starts fresh; `Submit` action drops the completed-attempt guard
- `Web/Views/Tests/Index.cshtml` — info banner updated; "Already Completed" disabled button replaced with "Retake Test" active button
- `App/ViewModels/TI/TiTestPageViewModel.cs` — `AlreadyAttempted` property removed; `LoadAsync` resumes IN_PROGRESS or starts fresh, never blocks
- `App/Views/TestsAndInterviews/TiTestPage.xaml.cs` — removed `AlreadyAttempted` navigation redirect
- `App/Views/TestsAndInterviews/TiMainTestPage.xaml` — info banner updated to say tests are replayable

**Leaderboard fix — best attempt per user:**
- `Library/Repositories/TestAttemptRepository.cs` — `FindValidAttemptsByTestIdAsync` now groups by user and takes only the highest-scoring attempt per user before ranking; previously all attempts appeared, inflating low scorers

**Multi-attempt support in Test Attempts tab:**
- `App/ViewModels/TestDashboardViewModel.cs` — replaced `ToDictionary(a => a.TestId)` (would throw with multiple attempts) with per-attempt loop plus test/question caches; now shows one card per attempt (full history)
- `Web/Clients/TestsApiClient.cs` — added `GetAttemptsByUserAsync(userId)` method
- `Web/Controllers/TestsController.cs` `Index` — replaced N+1 per-test attempt fetch with one bulk `GetAttemptsByUserAsync` call; uses `HashSet<int>` of completedTestIds to mark cards

**Expanded seed data (8 tests, 24 questions):**
- `Library/Persistence/Configurations/TestConfiguration.cs` — added tests 4–8: Python Fundamentals, Java Fundamentals, DevOps Basics, Data Science Basics, UI/UX Fundamentals
- `Library/Persistence/Configurations/QuestionConfiguration.cs` — added 15 questions (3 per new test, all SINGLE_CHOICE)
- New EF migration: `SeedAdditionalTests`

### 6. Tests — Bug Fixes: Score, Leaderboard, Tab Visibility (2026-06-05)

**Score = 0 on desktop (fixed):**
- `Library/Services/TestService.cs` — `SubmitAttemptAsync` now fetches the final score via `FindByIdAsync(attempt.Id)` instead of `FindByUserAndTestAsync(userId, testId)`. The old call returned null after submission because `FindByUserAndTestAsync` only returns IN_PROGRESS attempts.

**Leaderboard empty (fixed):**
- `Library/Services/DataProcessingService.cs` — removed the 3-month leaderboard validity window (`IsTestStillValidForLeaderboard`). All seeded tests (created 2026-01-01) were failing this check. Removed the constant and the method entirely.
- `Library/Services/DataProcessingService.cs` — `ProcessFinalizedAttemptAsync` now computes `PercentageScore` correctly as `(rawScore / maxPossibleScore) * 100` using the sum of question scores from `attempt.Answers`. The previous formula was an identity (`score / 100 * 100 = score`).
- `Library/Services/DataProcessingService.cs` — removed the `MaximumScore = 100m` cap on raw scores (was incorrectly rejecting valid attempts for tests with more than 10 questions).
- `Api/Controllers/TestsController.cs` — `SubmitAttempt` endpoint now calls `ILeaderboardService.RecalculateAsync(testId)` after each desktop submission so leaderboard entries are updated immediately.

**Test Attempts tab empty on web (fixed):**
- `Library/Repositories/TestAttemptRepository.cs` — `UpdateAsync` now checks `EntityState.Detached` and calls `context.TestAttempts.Update(entity)` before `SaveChangesAsync()`. Previously, entities created from DTOs (untracked) were passed to `UpdateAsync` which silently no-opped, leaving attempts stuck as IN_PROGRESS forever.
- `Web/Controllers/TestsController.cs` — `Submit` action now sets `attempt.IsValidated = true` before persisting, so web-submitted attempts qualify for leaderboard inclusion (the web flow bypasses `ProcessFinalizedAttemptAsync`).

---

## Build status

As of 2026-06-05 (post-bug-fixes), `dotnet build UBB_SE_2026_Jobs.slnx` produces **0 errors** (pre-existing warnings only; none introduced by this work).

---

## Key files reference

| File | Role |
|------|------|
| `Web/Controllers/AccountController.cs:143-172` | Login/register flow; sets `CompanyId` cookie claim for recruiters |
| `Web/Controllers/CompanyRecommendationsController.cs` | Ranked Applicants — now reads company from JWT claim |
| `Web/Controllers/CompanyStatusController.cs` | Applicant pipeline — now reads company from JWT claim; stubs removed |
| `Web/Controllers/MatchesController.cs` | Applicants list — now reads company from JWT claim |
| `Web/Controllers/ChatController.cs` | Chat — now reads company from JWT claim (no fallback) |
| `Library/Services/TestsAuthService.cs` | TI auth — JWT key now from `TiJwt:Key` config |
| `Api/appsettings.json` | Added `TiJwt:Key` |
| `Library/Domain/Applicant.cs` | TI evaluation data (grades); no FK to Match |
| `Library/Domain/Match.cs` | Application pipeline (Status, Feedback); no FK to Applicant |
| `Library/Repositories/RecruiterRepository.cs` | `GetCompanyIdForUserAsync(userId)` — used by ChatService internally |
