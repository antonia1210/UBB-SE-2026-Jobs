# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

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

### Tests & Interviews (TI) — separate API

The App's `Configuration` distinguishes two backends in `appsettings.json`:
- `Api:BaseUrl` (default `https://localhost:7134`) — the main PussyCats/Jobs API.
- `Api:TiBaseUrl` (default `http://localhost:5179`) — a **separate** "Tests & Interviews" API.

TI-only services (`ITiAuthService`, `ITiTestService`, `ITiApplicantService`, `ITiSlotsService`, etc., under `App/Services/TI`) point at `TiBaseUrl`. The TI **job catalog and skills** are owned by the main API, so they use `IJobService`/`ISkillService` on `BaseUrl` — see the comment block in `App.xaml.cs` before using a base URL for a TI call.

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
