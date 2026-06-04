
namespace UBB_SE_2026_Jobs.Api.Controllers;

    using Microsoft.AspNetCore.Mvc;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Controller used for grading operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class GradingController : ControllerBase
    {
        private readonly IGradingService gradingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradingController"/> class.
        /// </summary>
        /// <param name="gradingService">Injected grading service.</param>
        public GradingController(IGradingService gradingService)
        {
            this.gradingService = gradingService;
        }

        /// <summary>
        /// Grades a single choice TestQuestion.
        /// </summary>
        [HttpPost("single-choice")]
        public IActionResult GradeSingleChoice([FromBody] GradeRequest request)
        {
            this.gradingService.GradeSingleChoice(request.TestQuestion, request.Answer);
            return this.Ok(request.Answer);
        }

        /// <summary>
        /// Grades a multiple choice TestQuestion.
        /// </summary>
        [HttpPost("multiple-choice")]
        public IActionResult GradeMultipleChoice([FromBody] GradeRequest request)
        {
            this.gradingService.GradeMultipleChoice(request.TestQuestion, request.Answer);
            return this.Ok(request.Answer);
        }

        /// <summary>
        /// Grades a text TestQuestion.
        /// </summary>
        [HttpPost("text")]
        public IActionResult GradeText([FromBody] GradeRequest request)
        {
            this.gradingService.GradeText(request.TestQuestion, request.Answer);
            return this.Ok(request.Answer);
        }

        /// <summary>
        /// Grades a true/false TestQuestion.
        /// </summary>
        [HttpPost("true-false")]
        public IActionResult GradeTrueFalse([FromBody] GradeRequest request)
        {
            this.gradingService.GradeTrueFalse(request.TestQuestion, request.Answer);
            return this.Ok(request.Answer);
        }

        /// <summary>
        /// Calculates the final score.
        /// </summary>
        [HttpPost("final-score")]
        public ActionResult<float> CalculateFinalScore([FromBody] TestAttempt attempt)
        {
            var result = this.gradingService.CalculateFinalScore(attempt);
            return this.Ok(result);
        }
    }

    /// <summary>
    /// DTO used for grading requests.
    /// </summary>
    public class GradeRequest
    {
        /// <summary>
        /// Gets or sets TestQuestion.
        /// </summary>
        public required TestQuestion TestQuestion { get; set; }

        /// <summary>
        /// Gets or sets answer.
        /// </summary>
        public required Answer Answer { get; set; }
    }


