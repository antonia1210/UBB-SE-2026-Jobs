using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService questionService;

    public QuestionsController(IQuestionService questionService)
    {
        this.questionService = questionService;
    }

    [HttpGet("bytest/{testId}")]
    public async Task<ActionResult<List<QuestionDto>>> GetByTestId(int testId)
    {
        var questions = await questionService.GetQuestionsByTestIdAsync(testId);
        if (questions == null || !questions.Any())
            return NotFound($"No questions found for test ID {testId}.");

        return Ok(questions.Select(question => question.ToDto()).ToList());
    }

    [HttpGet("byposition/{positionId}")]
    public async Task<ActionResult<List<QuestionDto>>> GetByPosition(int positionId)
    {
        var questions = await questionService.GetInterviewQuestionsByPositionAsync(positionId);
        if (questions == null || !questions.Any())
            return NotFound($"No questions found for position ID {positionId}.");

        return Ok(questions.Select(question => question.ToDto()).ToList());
    }

    [HttpGet("{questionId}")]
    public async Task<ActionResult<QuestionDto>> Get(int questionId)
    {
        var testQuestion = await questionService.GetByIdAsync(questionId);
        if (testQuestion == null) return NotFound();
        return Ok(testQuestion.ToDto());
    }
}
