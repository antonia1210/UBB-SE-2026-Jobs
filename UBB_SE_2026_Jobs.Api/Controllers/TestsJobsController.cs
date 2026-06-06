using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsJobsController : ControllerBase
{
    private readonly ITestsJobsService testsJobsService;
    private readonly IRecruiterRepository recruiterRepository;

    public TestsJobsController(ITestsJobsService testsJobsService, IRecruiterRepository recruiterRepository)
    {
        this.testsJobsService = testsJobsService;
        this.recruiterRepository = recruiterRepository;
    }

    [HttpGet]
    public ActionResult<List<JobDto>> GetAllJobs([FromQuery] int? companyId = null)
    {
        IEnumerable<Job> jobs = this.testsJobsService.GetAllJobs();
        if (companyId.HasValue)
            jobs = jobs.Where(job => job.CompanyId == companyId.Value);
        return Ok(jobs.Select(job => job.ToDto()).ToList());
    }

    [HttpGet("skills")]
    public ActionResult<List<SkillDto>> GetAllSkills()
    {
        IReadOnlyList<Skill> skills = this.testsJobsService.GetAllSkills();
        return Ok(skills.Select(skill => skill.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<int>> AddJob([FromBody] AddJobDto addJobDto)
    {
        int userId = addJobDto.UserId;
        if (userId <= 0)
            return Unauthorized(new { message = "Unable to determine user identity." });

        int? companyId = await this.recruiterRepository.GetCompanyIdForUserAsync(userId);
        if (companyId == null)
            return Forbid("Recruiter profile not found for this user.");

        Job jobToAdd = addJobDto.Job.ToEntity();
        IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = addJobDto.SkillLinks
            .Select(jobSkill => (jobSkill.SkillId, jobSkill.RequiredPercentage))
            .ToList();

        int jobId = this.testsJobsService.AddJob(jobToAdd, companyId.Value, skillLinks);
        return Ok(jobId);
    }

    [HttpPut("{jobId}")]
    public ActionResult UpdateJob(int jobId, [FromBody] JobDto jobDto)
    {
        Job? existingJob = this.testsJobsService.GetJobById(jobId);
        if (existingJob == null)
            return NotFound(new { message = $"Job with ID {jobId} not found." });

        Job updatedJob = jobDto.ToEntity();
        IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = jobDto.JobSkills
            .Select(jobSkill => (jobSkill.SkillId, jobSkill.RequiredPercentage))
            .ToList();

        bool success = this.testsJobsService.UpdateJob(jobId, updatedJob, skillLinks);
        if (!success)
            return StatusCode(500, new { message = "An error occurred while updating the job." });

        return NoContent();
    }

    [HttpGet("{jobId}/applicant-count")]
    public ActionResult<int> GetApplicantCount(int jobId)
    {
        return Ok(this.testsJobsService.GetApplicantCount(jobId));
    }

    [HttpDelete("{jobId}")]
    public ActionResult DeleteJob(int jobId, [FromQuery] bool force = false)
    {
        var result = this.testsJobsService.DeleteJob(jobId, force);
        return result switch
        {
            JobDeleteResult.NotFound => NotFound(new { message = $"Job with ID {jobId} not found." }),
            JobDeleteResult.HasApplicants => Conflict(new { message = "This job has applicants. Confirm deletion to remove them along with the job." }),
            _ => NoContent(),
        };
    }

    [HttpGet("{jobId}/skills")]
    public ActionResult<List<JobSkillDto>> GetSkillsByJob(int jobId)
    {
        IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = this.testsJobsService.GetSkillsByJob(jobId);

        if (skillLinks is null || !skillLinks.Any())
            return NotFound($"No skills found for job ID {jobId}.");

        return Ok(skillLinks.Select(skillLink => new JobSkillDto
        {
            SkillId = skillLink.SkillId,
            JobId = jobId,
            RequiredPercentage = skillLink.RequiredPercentage,
        }).ToList());
    }
}
