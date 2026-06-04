namespace UBB_SE_2026_Jobs.Api.Controllers.TestsAndInterviews
{
    using Microsoft.AspNetCore.Mvc;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Dtos;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Mappers;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models.Core;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces;

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