using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Completeness;
using UBB_SE_2026_Jobs.Library.Services.CompletenessService;
using UBB_SE_2026_Jobs.Library.Services.CvParsing;
using UBB_SE_2026_Jobs.Library.Services.Documents;
using UBB_SE_2026_Jobs.Library.Services.Matches;
using UBB_SE_2026_Jobs.Library.Services.PdfExport;
using UBB_SE_2026_Jobs.Library.Services.SkillGapService;
using UBB_SE_2026_Jobs.Library.Services.SkillTests;
using UBB_SE_2026_Jobs.Library.Services.UserProfileService;
using UBB_SE_2026_Jobs.Library.Services.Users;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class PussyCatsUsersController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IMatchService matchService;
    private readonly IDocumentService documentService;
    private readonly IUserProfileService userProfileService;
    private readonly ISkillTestService skillTestService;
    private readonly ICvParsingService cvParsingService;
    private readonly ICompletenessService completenessService;
    private readonly ISkillGapService skillGapService;
    private readonly IPdfExportService pdfExportService;

    public PussyCatsUsersController(
        IUserService userService,
        IMatchService matchService,
        IDocumentService documentService,
        IUserProfileService userProfileService,
        ISkillTestService skillTestService,
        ICvParsingService cvParsingService,
        ICompletenessService completenessService,
        ISkillGapService skillGapService,
        IPdfExportService pdfExportService)
    {
        this.userService = userService;
        this.matchService = matchService;
        this.documentService = documentService;
        this.userProfileService = userProfileService;
        this.skillTestService = skillTestService;
        this.cvParsingService = cvParsingService;
        this.completenessService = completenessService;
        this.skillGapService = skillGapService;
        this.pdfExportService = pdfExportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await userService.GetAllAsync(cancellationToken));

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById(int userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await userService.GetByEmailAsync(email, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("exists-by-email/{email}")]
    public async Task<IActionResult> ExistsByEmail(string email, CancellationToken cancellationToken)
    {
        return Ok(await userService.ExistsWithEmailAsync(email, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] User user, CancellationToken cancellationToken)
    {
        user.UserId = 0;
        var savedUser = await userService.AddAsync(user, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { userId = savedUser.UserId }, savedUser);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(int userId, [FromBody] User user, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        user.UserId = userId;
        await userService.UpdateAsync(user, cancellationToken);
        return NoContent();
    }

    [HttpPut("{userId}/profile")]
    public async Task<IActionResult> SaveProfile(int userId, [FromBody] User user, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        await userProfileService.SaveAsync(userId, user, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> Remove(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        await userService.RemoveAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{userId}/active")]
    public async Task<IActionResult> UpdateActive(int userId, [FromBody] UpdateActiveRequest updateActiveRequest, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        await userService.SetActiveAsync(userId, updateActiveRequest.IsActive, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{userId}/profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture(int userId, [FromBody] UpdateProfilePictureRequest updateProfilePictureRequest, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        await userService.SetProfilePicturePathAsync(userId, updateProfilePictureRequest.Path ?? string.Empty, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId}/profile-picture")]
    public async Task<IActionResult> RemoveProfilePicture(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        await userService.SetProfilePicturePathAsync(userId, string.Empty, cancellationToken);
        return NoContent();
    }

    [HttpGet("{userId}/matches")]
    public async Task<IActionResult> GetMatches(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        return Ok(await matchService.GetMatchesForUserAsync(userId, cancellationToken));
    }

    [HttpGet("{userId}/documents")]
    public async Task<IActionResult> GetDocuments(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        return Ok(await documentService.GetDocumentsByUserIdAsync(userId, cancellationToken));
    }

    [HttpPost("{userId}/cv")]
    public async Task<IActionResult> UploadCv(
        int userId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return NotFound();

        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "No file uploaded." });

        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            var fileType = Path.GetExtension(file.FileName);
            var parsedUser = cvParsingService.ParseCvFile(content, fileType);
            await userProfileService.SaveAsync(userId, parsedUser, cancellationToken);
            return Ok(parsedUser);
        }
        catch (Exception exception)
        {
            return BadRequest(new { detail = exception.Message });
        }
    }

    [HttpGet("{userId}/compatibility")]
    public IActionResult GetCompatibility(int userId) =>
        Problem("Compatibility computation is wired in Phase 5.", statusCode: 501);

    [HttpGet("{userId}/completeness")]
    public async Task<IActionResult> GetCompleteness(int userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null) return NotFound();

        return Ok(new
        {
            Percentage = completenessService.CalculateCompleteness(user),
            NextPrompt = completenessService.GetNextEmptyFieldPrompt(user),
        });
    }

    [HttpGet("{userId}/skill-gap")]
    public async Task<IActionResult> GetSkillGap(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();

        return Ok(new
        {
            Summary = await skillGapService.GetSummaryAsync(userId, cancellationToken),
            MissingSkills = await skillGapService.GetMissingSkillsAsync(userId, cancellationToken),
            UnderscoredSkills = await skillGapService.GetUnderscoredSkillsAsync(userId, cancellationToken),
        });
    }

    [HttpGet("{userId}/experience")]
    public async Task<IActionResult> RecalculateExperience(int userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null) return NotFound();

        int experiencePoints = await userProfileService.RecalculateLevelAsync(user, cancellationToken);
        await users.UpdateAsync(user, cancellationToken);

        return Ok(new { TotalExperiencePoints = experiencePoints, CurrentLevel = user.CurrentLevel });

    }

    [HttpGet("{userId}/skill-tests")]
    public async Task<IActionResult> GetSkillTests(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        return Ok(await skillTestService.GetTestsForUserAsync(userId, cancellationToken));
    }

    [HttpGet("{userId}/is-active")]
    public async Task<IActionResult> IsProfileAvailable(int userId, CancellationToken cancellationToken)
    {
        if (await userService.GetByIdAsync(userId, cancellationToken) is null)
            return NotFound();
        return Ok(await userProfileService.IsProfileAvailableAsync(userId, cancellationToken));
    }

    [HttpGet("{userId}/parsed-cv")]
    public async Task<IActionResult> GetParsedCv(int userId, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(userId, cancellationToken);
        if (user is null) return NotFound();
        string parsedCv = Helpers.GenerateParsedCvText(user);
        return Ok(new { ParsedCv = parsedCv });
    }

    [HttpGet("{userId}/cv/html")]
    public async Task<IActionResult> GetCvHtml(int userId, CancellationToken cancellationToken)
    {
        var user = await userProfileService.GetProfileAsync(userId, cancellationToken);
        if (user is null)
            return NotFound();

        var html = await pdfExportService.RenderHtmlAsync(user);
        return Content(html, "text/html");
    }

    [HttpGet("{userId}/cv/pdf")]
    public async Task<IActionResult> DownloadCvPdf(int userId, CancellationToken cancellationToken)
    {
        var user = await userProfileService.GetProfileAsync(userId, cancellationToken);
        if (user is null)
            return NotFound();

        var pdfBytes = await pdfExportService.GeneratePdfAsync(user);
        return File(pdfBytes, "application/pdf", $"{user.FirstName}_{user.LastName}_CV.pdf");
    }

    public record UpdateActiveRequest(bool IsActive);
    public record UpdateProfilePictureRequest(string? Path);
}