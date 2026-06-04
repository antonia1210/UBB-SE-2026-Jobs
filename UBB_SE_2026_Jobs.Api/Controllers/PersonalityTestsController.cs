using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Domain.Enums;
using UBB_SE_2026_Jobs.Library.Services.PersonalityTestService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/personality-tests")]
public class PersonalityTestsController : ControllerBase
{
    private readonly IPersonalityTestService personalityTestService;

    public PersonalityTestsController(IPersonalityTestService personalityTestService)
    {
        this.personalityTestService = personalityTestService;
    }

    /// <summary>
    /// Returns all personality test questions.
    /// </summary>
    [HttpGet("questions")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonalityQuestion>), StatusCodes.Status200OK)]
    public IActionResult GetQuestions()
    {
        var questions = PersonalityTestService.LoadQuestions();
        return Ok(questions);
    }

    /// <summary>
    /// Returns the personality test result for a given user.
    /// </summary>
    [HttpGet("users/{userId:int}")]
    public async Task<IActionResult> GetByUserId(
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        var result = await personalityTestService.GetByUserIdAsync(userId, cancellationToken);
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Calculates trait scores from the provided answers.
    /// </summary>
    [HttpPost("trait-scores")]
    public IActionResult CalculateTraitScores(
        [FromBody] CalculateTraitScoresRequest request)
    {
        var questions = PersonalityTestService.LoadQuestions()
            .ToDictionary(PersonalityQuestion => PersonalityQuestion.QuestionText);

        var answers = new Dictionary<PersonalityQuestion, AnswerValue>();
        foreach (var (questionText, answerValue) in request.Answers)
        {
            if (!questions.TryGetValue(questionText, out var PersonalityQuestion))
                return BadRequest($"Unknown PersonalityQuestion: '{questionText}'.");

            answers[PersonalityQuestion] = answerValue;
        }

        var traitScores = personalityTestService.CalculateTraitScores(answers);
        return Ok(traitScores);
    }

    /// <summary>
    /// Calculates role scores from the provided trait scores.
    /// </summary>
    [HttpPost("role-scores")]
    public IActionResult CalculateRoleScores(
        [FromBody] IReadOnlyDictionary<TraitType, double> traitScores)
    {
        var roleScores = personalityTestService.CalculateRoleScores(traitScores);
        return Ok(roleScores);
    }

    /// <summary>
    /// Returns the top N roles from the provided role scores.
    /// </summary>
    [HttpPost("role-scores/top")]
    public IActionResult GetTopRoles(
        [FromBody] GetTopRolesRequest request)
    {
        if (request.Count <= 0)
            return BadRequest("Count must be greater than zero.");

        var topRoles = personalityTestService.GetTopRoles(request.RoleScores, request.Count);
        return Ok(topRoles);
    }

    /// <summary>
    /// Saves the personality test result for a user.
    /// </summary>
    [HttpPost("users/{userId:int}")]
    public async Task<IActionResult> SaveResult(
        [FromRoute] int userId,
        [FromBody] SaveResultRequest request,
        CancellationToken cancellationToken)
    {
        var questions = PersonalityTestService.LoadQuestions()
            .ToDictionary(PersonalityQuestion => PersonalityQuestion.QuestionText);

        var answers = new Dictionary<PersonalityQuestion, AnswerValue>();
        foreach (var (questionText, answerValue) in request.Answers)
        {
            if (!questions.TryGetValue(questionText, out var PersonalityQuestion))
                return BadRequest($"Unknown PersonalityQuestion: '{questionText}'.");

            answers[PersonalityQuestion] = answerValue;
        }

        await personalityTestService.SaveResultAsync(userId, answers, request.SelectedRole, cancellationToken);
        return NoContent();
    }
}


public record CalculateTraitScoresRequest(IReadOnlyDictionary<string, AnswerValue> Answers);

public record GetTopRolesRequest(IReadOnlyDictionary<JobRole, double> RoleScores, int Count);

public record SaveResultRequest(IReadOnlyDictionary<string, AnswerValue> Answers, JobRole SelectedRole);

