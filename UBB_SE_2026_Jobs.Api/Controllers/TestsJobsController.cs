
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Services;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class TestsJobsController : ControllerBase
    {
        private readonly ITestsJobsService _service;
        private readonly JobsDbContext _dbContext;

        public TestsJobsController(ITestsJobsService service, JobsDbContext dbContext)
        {
            this._service = service;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<List<JobDto>> GetAllJobs()
        {
            IEnumerable<Job> jobs = this._service.GetAllJobs();

            return Ok(jobs.Select(j => j.ToDto()).ToList());
        }

        [HttpGet("skills")]
        public ActionResult<List<SkillDto>> GetAllSkills()
        {
            IReadOnlyList<Skill> skills = this._service.GetAllSkills();

            return Ok(skills.Select(s => s.ToDto()).ToList());
        }

        [HttpPost]
        public ActionResult<int> AddJob([FromBody] AddJobDto dto)
        {
            int userId = dto.UserId;
            if (userId <= 0)
            {
                return Unauthorized(new { message = "Unable to determine user identity." });
            }

            Recruiter? recruiter = this._dbContext.Recruiters.FirstOrDefault(r => r.UserId == userId);
            if (recruiter == null)
            {
                return Forbid("Recruiter profile not found for this user.");
            }

            Job Job = dto.Job.ToEntity();
            IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = dto.SkillLinks
                .Select(s => (s.SkillId, s.RequiredPercentage))
                .ToList();

            int jobId = this._service.AddJob(Job, recruiter.CompanyId, skillLinks);

            return Ok(jobId);
        }

        [HttpPut("{jobId}")]
        public ActionResult UpdateJob(int jobId, [FromBody] JobDto dto)
        {
            Job? existingJob = this._service.GetJobById(jobId);
            if (existingJob == null)
            {
                return NotFound(new { message = $"Job with ID {jobId} not found." });
            }

            Job updatedJob = dto.ToEntity();
            IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = dto.JobSkills
                .Select(s => (s.SkillId, s.RequiredPercentage))
                .ToList();

            bool success = this._service.UpdateJob(jobId, updatedJob, skillLinks);
            if (!success)
            {
                return StatusCode(500, new { message = "An error occurred while updating the job." });
            }

            return NoContent();
        }

        [HttpGet("{jobId}/applicant-count")]
        public ActionResult<int> GetApplicantCount(int jobId)
        {
            return Ok(this._service.GetApplicantCount(jobId));
        }

        [HttpDelete("{jobId}")]
        public ActionResult DeleteJob(int jobId, [FromQuery] bool force = false)
        {
            JobDeleteResult result = this._service.DeleteJob(jobId, force);

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
            IReadOnlyList<(int SkillId, int RequiredPercentage)> skillLinks = this._service.GetSkillsByJob(jobId);

            if (skillLinks is null || !skillLinks.Any())
                return NotFound($"No skills found for job ID {jobId}.");

            return Ok(skillLinks.Select(s => new JobSkillDto
            {
                SkillId = s.SkillId,
                JobId = jobId,
                RequiredPercentage = s.RequiredPercentage,
            }).ToList());
        }
    }


