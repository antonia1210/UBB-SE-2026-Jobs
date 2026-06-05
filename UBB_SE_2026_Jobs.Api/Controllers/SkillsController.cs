using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.Skills;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/skills")]
public class SkillsController : ControllerBase
{
    private readonly ISkillService skillService;

    public SkillsController(ISkillService skillService)
    {
        this.skillService = skillService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await skillService.GetAllAsync(cancellationToken));

    [HttpGet("{skillId}")]
    public async Task<IActionResult> GetById(int skillId, CancellationToken cancellationToken)
    {
        var skill = await skillService.GetByIdAsync(skillId, cancellationToken);
        return skill is null ? NotFound() : Ok(skill);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Skill skill, CancellationToken cancellationToken)
    {
        skill.SkillId = 0;
        var savedSkill = await skillService.AddAsync(skill, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { skillId = savedSkill.SkillId }, savedSkill);
    }

    [HttpPut("{skillId}")]
    public async Task<IActionResult> Update(int skillId, [FromBody] Skill skill, CancellationToken cancellationToken)
    {
        if (await skillService.GetByIdAsync(skillId, cancellationToken) is null)
            return NotFound();
        skill.SkillId = skillId;
        await skillService.UpdateAsync(skill, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{skillId}")]
    public async Task<IActionResult> Remove(int skillId, CancellationToken cancellationToken)
    {
        if (await skillService.GetByIdAsync(skillId, cancellationToken) is null)
            return NotFound();
        try
        {
            await skillService.RemoveAsync(skillId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }
}