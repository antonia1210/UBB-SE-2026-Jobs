
$controllersDir = "E:\ISS\Jobs_ISS_Final\UBB_SE_2026_Jobs.Api\Controllers"
$files = Get-ChildItem -Path "$controllersDir\*.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $changed = $false

    # 1. Namespace - standardize to file-scoped
    if ($content -match 'namespace UBB_SE_2026_Jobs\.Api\.Controllers\s*\{') {
        $content = $content -replace 'namespace UBB_SE_2026_Jobs\.Api\.Controllers\s*\{', "namespace UBB_SE_2026_Jobs.Api.Controllers;"
        # Remove the last closing brace for the namespace
        $content = $content -replace '\}\s*$', ""
        $changed = $true
    }

    # 2. Usings
    $usings = @(
        @('using PussyCats\.Library', 'using UBB_SE_2026_Jobs.Library'),
        @('using Tests_and_Interviews_API', 'using UBB_SE_2026_Jobs.Library'),
        @('using UBB_SE_2026_Jobs\.Library\.Data', 'using UBB_SE_2026_Jobs.Library.Persistence'),
        @('using UBB_SE_2026_Jobs\.Api\.Dtos', 'using UBB_SE_2026_Jobs.Library.DTOs'),
        @('using UBB_SE_2026_Jobs\.Api\.DTOs', 'using UBB_SE_2026_Jobs.Library.DTOs'),
        @('using UBB_SE_2026_Jobs\.Api\.Mappers', 'using UBB_SE_2026_Jobs.Library.Mappers'),
        @('using UBB_SE_2026_Jobs\.Api\.Models', 'using UBB_SE_2026_Jobs.Library.Domain'),
        @('using UBB_SE_2026_Jobs\.Api\.Services', 'using UBB_SE_2026_Jobs.Library.Services'),
        @('using UBB_SE_2026_Jobs\.Api\.Data', 'using UBB_SE_2026_Jobs.Library.Persistence'),
        @('using UBB_SE_2026_Jobs\.Api\.Services\.Interfaces', 'using UBB_SE_2026_Jobs.Library.Services'),
        @('using Tests_and_Interviews\.Mappers', 'using UBB_SE_2026_Jobs.Library.Mappers')
    )

    foreach ($u in $usings) {
        if ($content -match $u[0]) {
            $content = $content -replace $u[0], $u[1]
            $changed = $true
        }
    }

    # Add Microsoft.AspNetCore.Http if needed
    if (($content -match 'IFormFile' -or $content -match 'HttpRequest') -and $content -notmatch 'using Microsoft\.AspNetCore\.Http;') {
        $content = "using Microsoft.AspNetCore.Http;`r`n" + $content
        $changed = $true
    }

    # 3. Context
    if ($content -match 'AppDbContext' -or $content -match 'PussyCatsDbContext') {
        $content = $content -replace 'AppDbContext', 'JobsDbContext'
        $content = $content -replace 'PussyCatsDbContext', 'JobsDbContext'
        $changed = $true
    }

    # 4. Service Renames
    if ($file.Name -like "PussyCats*") {
        $content = $content -replace '\bICompanyService\b', 'IPussyCatsCompanyService'
        $content = $content -replace '\bIJobService\b', 'IPussyCatsJobService'
        $content = $content -replace '\bIAuthService\b', 'IPussyCatsAuthService'
    } else {
        $content = $content -replace '\bIAuthService\b', 'ITestsAuthService'
        $content = $content -replace '\bICompanyService\b', 'ITestsCompanyService'
        $content = $content -replace '\bIJobService\b', 'ITestsJobsService'
        $content = $content -replace '\bIJobsService\b', 'ITestsJobsService'
        $content = $content -replace '\bICompanyRepo\b', 'ITestsCompanyRepository'
        $content = $content -replace '\bIJobsRepository\b', 'ITestsJobsRepository'
    }

    # 5. Model Renames
    $content = $content -replace '\bJobPosting\b', 'Job'
    
    if ($file.Name -eq "PersonalityTestsController.cs") {
        # Keep PersonalityQuestion
    } else {
        $content = $content -replace '\bQuestion\b', 'TestQuestion'
    }

    # 6. Class/Constructor name
    $expectedName = $file.BaseName
    # Try to find the class name
    if ($content -match 'public class (\w+)') {
        $actualName = $matches[1]
        if ($actualName -ne $expectedName) {
            $content = $content -replace "\bclass $actualName\b", "class $expectedName"
            $content = $content -replace "\b$actualName\(", "$expectedName("
            $changed = $true
        }
    }

    if ($changed) {
        Set-Content $file.FullName $content
    }
}
