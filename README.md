# UBB-SE-2026-Jobs

Configure Multiple Startup Projects

1. Right-click the **solution** in Solution Explorer → **Set Startup Projects…**
2. Select **Multiple startup projects** and set the following to **Start**:

| Project | Action |
|---------|--------|
| `UBB_SE_2026_Jobs.Api` | Start |
| `UBB_SE_2026_Jobs.App` | Start |
| `UBB_SE_2026_Jobs.Web` | Start |
| `UBB_SE_2026_Jobs.Library` | None |
| `UBB_SE_2026_Jobs.Tests` | None |

## Configure Local Connections and Environment Settings

Before running the projects, you must update the local configurations to match your local database and environment ports:

### 1. Database Connection String
Open **`appsettings.json`** and **`appsettings.Development.json`** inside the **`UBB_SE_2026_Jobs.Api`**  and **`helpers/Env.cs`** inside the **`UBB_SE_2026_Jobs.Library`** project and adjust the connection details to match your local setup:

```json
"ConnectionStrings": {
  "JobsDb": "Server=YOUR_SERVER;Database=Jobs_ISS_Final;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True"
},
```

### 2. Deploy configuration

Make sure to have WireGuard activated!

Replace urls of api `https://localhost:7168/`, `http://localhost:5179/` with 
	`http://172.30.242.79/` or `http://172.30.242.19/` (backup server) in App and Web projects.