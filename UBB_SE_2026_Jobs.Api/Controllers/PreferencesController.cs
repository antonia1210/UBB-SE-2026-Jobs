using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Preferences;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/preferences")]
public class PreferencesController : ControllerBase
{
    private readonly IPreferenceService preferenceService;

    public PreferencesController(IPreferenceService preferenceService)
    {
        this.preferenceService = preferenceService;
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserPreferences>> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var userPreferences = await preferenceService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(userPreferences);
    }

    [HttpPut("{userId:int}")]
    public async Task<IActionResult> Save(int userId, [FromBody] SavePreferencesRequest savePreferencesRequest, CancellationToken cancellationToken)
    {
        try
        {
            await preferenceService.SavePreferencesAsync(userId, savePreferencesRequest.Roles, savePreferencesRequest.WorkMode, savePreferencesRequest.Location, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException argumentException)
        {
            return ValidationProblem(argumentException.Message);
        }
    }

    [HttpGet("locations")]
    public async Task<ActionResult<IReadOnlyList<string>>> SearchLocations([FromQuery] string locationsQuery, CancellationToken cancellationToken)
    {
        var matchingLocations = await preferenceService.SearchLocationsAsync(locationsQuery ?? string.Empty, cancellationToken);
        return Ok(matchingLocations);
    }

    public sealed record SavePreferencesRequest(
        IReadOnlyList<JobRole> Roles,
        WorkMode WorkMode,
        string Location);
}