namespace UBB_SE_2026_Jobs.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Services.Portal;

    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            this._service = service;
        }

        [HttpPost("process/{jobId}")]
        public async Task<ActionResult> ProcessPayment(int jobId, [FromQuery] int paymentAmount)
        {
            await this._service.ProcessPaymentAsync(jobId, paymentAmount);
            return Ok();
        }

        [HttpPut("{jobId}")]
        public ActionResult UpdateJobPayment(int jobId, [FromQuery] int paymentAmount)
        {
            this._service.UpdateJobPayment(jobId, paymentAmount);
            return Ok();
        }

        [HttpGet("paid")]
        public ActionResult<List<JobPaymentInfoDto>> GetPaidJobs([FromQuery] string jobType, [FromQuery] string experienceLevel)
        {
            List<JobPaymentInfo> jobs = this._service.GetPaidJobs(jobType, experienceLevel);

            if (jobs is null || !jobs.Any())
                return NotFound($"No paid jobs found for type '{jobType}' and experience level '{experienceLevel}'.");

            return Ok(jobs.Select(j => j.ToDto()).ToList());
        }
    }
}