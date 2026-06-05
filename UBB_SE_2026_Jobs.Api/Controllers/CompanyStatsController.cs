namespace UBB_SE_2026_Jobs.Api.Controllers;

    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class CompanyStatsController : ControllerBase
    {
        private readonly ICompanyStatsService _service;

        public CompanyStatsController(ICompanyStatsService service)
        {
            this._service = service;
        }

        [HttpGet("{companyId}/skills/top3")]
        public async Task<ActionResult> GetSkillsTop3(int companyId)
        {
            var (skillNames, percents) = await this._service.GetSkillsTop3Async(companyId);

            if (skillNames == null || !skillNames.Any())
                return NotFound($"No skills found for company ID {companyId}.");

            return Ok(new { SkillNames = skillNames, Percents = percents });
        }

        [HttpGet("{companyId}/applicantsmessage")]
        public async Task<ActionResult<string>> GetApplicantsMessage(int companyId)
        {
            string message = await this._service.ApplicantsMessageAsync(companyId);
            return Ok(message);
        }
    }


