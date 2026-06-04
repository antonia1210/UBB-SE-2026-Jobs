namespace UBB_SE_2026_Jobs.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Services.Portal;

    [Route("api/[controller]")]
    [ApiController]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerService _service;

        public AnswersController(IAnswerService service)
        {
            this._service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Save([FromBody] AnswerDto dto)
        {
            await this._service.SaveAsync(dto.ToEntity());

            return Ok();
        }

        [HttpGet("byattempt/{attemptId}")]
        public async Task<ActionResult<List<AnswerDto>>> FindByAttempt(int attemptId)
        {
            List<Answer> answers = await this._service.FindByAttemptAsync(attemptId);

            if (answers is null || !answers.Any())
                return NotFound($"No answers found for attempt ID {attemptId}.");

            return Ok(answers.Select(a => a.ToDto()).ToList());
        }
    }
}