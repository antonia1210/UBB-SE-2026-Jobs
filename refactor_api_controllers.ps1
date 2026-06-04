$dir = "E:\ISS\Jobs_ISS_Final\UBB_SE_2026_Jobs.Api\Controllers"
$files = Get-ChildItem $dir -Filter *.cs

foreach ($file in $files) {
    Write-Host "Processing $($file.Name)..."
    $content = Get-Content $file.FullName -Raw
    
    # 1. Namespace fix (from block to file-scoped)
    if ($content -match '(?m)^namespace\s+UBB_SE_2026_Jobs\.Api\.Controllers\s*\{') {
        $content = $content -replace '(?m)^namespace\s+UBB_SE_2026_Jobs\.Api\.Controllers\s*\{', "namespace UBB_SE_2026_Jobs.Api.Controllers;`r`n"
        # Find the last closing brace and remove it
        $lastBraceIndex = $content.LastIndexOf('}')
        if ($lastBraceIndex -ge 0) {
            $content = $content.Remove($lastBraceIndex, 1)
        }
    }

    # 2. Using replacements
    $content = $content -replace 'using PussyCats\.Library', 'using UBB_SE_2026_Jobs.Library'
    $content = $content -replace 'using Tests_and_Interviews_API', 'using UBB_SE_2026_Jobs.Library'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Library\.Data', 'using UBB_SE_2026_Jobs.Library.Persistence'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Api\.Dtos', 'using UBB_SE_2026_Jobs.Library.DTOs'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Api\.Mappers', 'using UBB_SE_2026_Jobs.Library.Mappers'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Api\.Models', 'using UBB_SE_2026_Jobs.Library.Domain'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Api\.Services', 'using UBB_SE_2026_Jobs.Library.Services'
    $content = $content -replace 'using UBB_SE_2026_Jobs\.Api\.Data', 'using UBB_SE_2026_Jobs.Library.Persistence'
    $content = $content -replace 'using Tests_and_Interviews\.Mappers', 'using UBB_SE_2026_Jobs.Library.Mappers'

    # 3. Context Renames
    $content = $content -replace 'AppDbContext', 'JobsDbContext'
    $content = $content -replace 'PussyCatsDbContext', 'JobsDbContext'

    # 4. Model Renames
    $content = $content -replace 'JobPosting', 'Job'
    $content = $content -replace 'JobPostingDto', 'JobDto'

    if ($file.Name -match "Personality") {
        $content = $content -replace '\bQuestion\b', 'PersonalityQuestion'
    } else {
        $content = $content -replace '\bQuestion\b', 'TestQuestion'
    }

    # 5. Service/Repo Renames
    if ($file.Name -match "PussyCats") {
        $content = $content -replace 'ICompanyService', 'IPussyCatsCompanyService'
        $content = $content -replace 'IJobService', 'IPussyCatsJobService'
        $content = $content -replace 'IAuthService', 'IPussyCatsAuthService'
        $content = $content -replace 'ICompanyRepository', 'IPussyCatsCompanyRepository'
        $content = $content -replace 'IJobRepository', 'IPussyCatsJobRepository'
    } else {
        # Tests* and others
        # Use word boundaries for replacements to avoid partial matches
        $content = $content -replace '\bIAuthService\b', 'ITestsAuthService'
        $content = $content -replace '\bICompanyService\b', 'ITestsCompanyService'
        $content = $content -replace '\bIJobService\b', 'ITestsJobsService'
        $content = $content -replace '\bIJobsService\b', 'ITestsJobsService'
        $content = $content -replace '\bICompanyRepo\b', 'ITestsCompanyRepository'
        $content = $content -replace '\bIJobsRepository\b', 'ITestsJobsRepository'
    }

    # 6. Ensure Interfaces Usings
    $needsInterfaces = $content -match 'ITests|IPussyCats'
    if ($needsInterfaces) {
        if ($content -notmatch 'using UBB_SE_2026_Jobs\.Library\.Repositories\.Interfaces;') {
             $content = $content -replace '(namespace UBB_SE_2026_Jobs\.Api\.Controllers;)', "$1`r`nusing UBB_SE_2026_Jobs.Library.Repositories.Interfaces;"
        }
        if ($content -notmatch 'using UBB_SE_2026_Jobs\.Library\.Services\.Interfaces;') {
             $content = $content -replace '(namespace UBB_SE_2026_Jobs\.Api\.Controllers;)', "$1`r`nusing UBB_SE_2026_Jobs.Library.Services.Interfaces;"
        }
    }

    # 7. Http Usings
    if ($content -match 'IFormFile|HttpRequest') {
        if ($content -notmatch 'using Microsoft\.AspNetCore\.Http;') {
            $content = "using Microsoft.AspNetCore.Http;`r`n" + $content
        }
    }

    # 8. Class name / constructor name fix (already mostly correct if files were renamed before)
    # But let's make sure class names match filename (without .cs)
    $className = $file.BaseName
    # This is harder to do with regex without matching the whole file, but let's try a simple one
    # public class OldName : ControllerBase -> public class NewName : ControllerBase
    $content = $content -replace 'public class \w+ : ControllerBase', "public class $className : ControllerBase"
    # Constructor rename: public OldName( -> public NewName(
    # This might match too much if not careful, but usually it's unique enough
    $content = $content -replace 'public \w+\(', "public $className("

    Set-Content $file.FullName $content -NoNewline
}
