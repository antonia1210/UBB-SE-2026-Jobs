using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.Services.Jobs;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/jobs")]
public class PussyCatsJobsController : ControllerBase
{
    private readonly IPussyCatsJobService jobService;
    private readonly ITestsJobsService testsJobs;

    public PussyCatsJobsController(IPussyCatsJobService jobService, ITestsJobsService testsJobs)
    {
        this.jobService = jobService;
        this.testsJobs = testsJobs;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? companyId, CancellationToken cancellationToken)
    {
        if (companyId.HasValue)
            return Ok(await jobService.GetByCompanyIdAsync(companyId.Value, cancellationToken));
        return Ok(await jobService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{jobId}")]
    public async Task<IActionResult> GetById(int jobId, CancellationToken cancellationToken)
    {
        var job = await jobService.GetByIdAsync(jobId, cancellationToken);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Job job, CancellationToken cancellationToken)
    {
        job.JobId = 0;
        var savedJob = await jobService.AddAsync(job, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { jobId = savedJob.JobId }, savedJob);
    }

    [HttpPut("{jobId}")]
    public async Task<IActionResult> Update(int jobId, [FromBody] Job job, CancellationToken cancellationToken)
    {
        if (await jobService.GetByIdAsync(jobId, cancellationToken) is null)
            return NotFound();
        job.JobId = jobId;
        await jobService.UpdateAsync(job, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}/applicant-count")]
    public IActionResult GetApplicantCount(int id)
    {
        return Ok(testsJobs.GetApplicantCount(id));
    }

    [HttpDelete("{id}")]
    public IActionResult Remove(int id, [FromQuery] bool force = false)
    {
        JobDeleteResult result = testsJobs.DeleteJob(id, force);

        return result switch
        {
            JobDeleteResult.NotFound => NotFound(),
            JobDeleteResult.HasApplicants => Conflict(new { message = "This job has applicants. Confirm deletion to remove them along with the job." }),
            _ => NoContent(),
        };
    }
}
