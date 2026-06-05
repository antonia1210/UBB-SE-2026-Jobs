using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Repositories.Skills;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/skill-groups")]
public class SkillGroupsController : ControllerBase
{
    private readonly ISkillGroupRepository skillGroupRepository;

    public SkillGroupsController(ISkillGroupRepository skillGroupRepository)
    {
        this.skillGroupRepository = skillGroupRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] JobRole? jobRole, CancellationToken cancellationToken)
    {
        if (jobRole.HasValue)
            return Ok(await skillGroupRepository.GetByJobRoleAsync(jobRole.Value, cancellationToken));

        return Ok(await skillGroupRepository.GetAllAsync(cancellationToken));
    }
}