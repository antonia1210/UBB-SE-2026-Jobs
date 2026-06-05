using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.UserSkillService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/users/{userId}/skills")]
public class UserSkillsController : ControllerBase
{
    private readonly IUserSkillService userSkillService;

    public UserSkillsController(IUserSkillService userSkillService)
    {
        this.userSkillService = userSkillService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
        => Ok(await userSkillService.GetByUserIdAsync(userId, cancellationToken));

    [HttpGet("verified")]
    public async Task<IActionResult> GetVerifiedByUserId(int userId, CancellationToken cancellationToken)
        => Ok(await userSkillService.GetVerifiedByUserIdAsync(userId, cancellationToken));

    [HttpGet("{skillId}")]
    public async Task<IActionResult> GetBySkillId(int userId, int skillId, CancellationToken cancellationToken)
    {
        var userSkill = await userSkillService.GetAsync(userId, skillId, cancellationToken);
        return userSkill is null ? NotFound() : Ok(userSkill);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int userId, [FromBody] UserSkill userSkill, CancellationToken cancellationToken)
    {
        userSkill.User = new User { UserId = userId };
        var savedUserSkill = await userSkillService.AddAsync(userSkill, cancellationToken);
        return CreatedAtAction(nameof(GetBySkillId), new { userId, skillId = savedUserSkill.Skill.SkillId }, savedUserSkill);
    }

    [HttpPut("{skillId}")]
    public async Task<IActionResult> Update(int userId, int skillId, [FromBody] UserSkill userSkill, CancellationToken cancellationToken)
    {
        if (await userSkillService.GetAsync(userId, skillId, cancellationToken) is null)
            return NotFound();
        userSkill.User = new User { UserId = userId };
        userSkill.Skill = new Skill { SkillId = skillId };
        await userSkillService.UpdateAsync(userSkill, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{skillId}/score")]
    public async Task<IActionResult> UpdateScore(int userId, int skillId, [FromBody] UpdateScoreRequest body, CancellationToken cancellationToken)
    {
        if (await userSkillService.GetAsync(userId, skillId, cancellationToken) is null)
            return NotFound();
        await userSkillService.UpdateScoreAsync(userId, skillId, body.Score, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{skillId}")]
    public async Task<IActionResult> Remove(int userId, int skillId, CancellationToken cancellationToken)
    {
        if (await userSkillService.GetAsync(userId, skillId, cancellationToken) is null)
            return NotFound();
        await userSkillService.RemoveAsync(userId, skillId, cancellationToken);
        return NoContent();
    }

    public record UpdateScoreRequest(int Score);
}