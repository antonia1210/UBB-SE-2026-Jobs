using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.Services.Jobs;


using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

[Authorize]
[ApiController]
[Route("api/jobs")]
public class PussyCatsJobsController : ControllerBase
{
    private readonly IPussyCatsJobService jobs;
    private readonly ITestsJobsService testsJobs;

    public PussyCatsJobsController(IPussyCatsJobService jobs, ITestsJobsService testsJobs)
    {
        this.jobs = jobs;
        this.testsJobs = testsJobs;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? companyId, CancellationToken cancellationToken)
    {
        if (companyId.HasValue)
            return Ok(await jobs.GetByCompanyIdAsync(companyId.Value, cancellationToken));
        return Ok(await jobs.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var job = await jobs.GetByIdAsync(id, cancellationToken);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Job job, CancellationToken cancellationToken)
    {
        job.JobId = 0;
        var saved = await jobs.AddAsync(job, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = saved.JobId }, saved);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Job job, CancellationToken cancellationToken)
    {
        if (await jobs.GetByIdAsync(id, cancellationToken) is null)
            return NotFound();
        job.JobId = id;
        await jobs.UpdateAsync(job, cancellationToken);
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

