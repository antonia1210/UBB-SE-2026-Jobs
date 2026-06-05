using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService questionService;
    private readonly JobsDbContext databaseContext;

    public QuestionsController(IQuestionService questionService, JobsDbContext databaseContext)
    {
        this.questionService = questionService;
        this.databaseContext = databaseContext;
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
        var testQuestion = await databaseContext.Questions
            .Include(question => question.Answers)
            .FirstOrDefaultAsync(question => question.Id == questionId);
        if (testQuestion == null) return NotFound();
        return Ok(testQuestion.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create([FromBody] QuestionDto questionDto)
    {
        var questionEntity = questionDto.ToEntity();
        databaseContext.Questions.Add(questionEntity);
        await databaseContext.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { questionId = questionEntity.Id }, questionEntity.ToDto());
    }

    [HttpPut("{questionId}")]
    public async Task<IActionResult> Update(int questionId, [FromBody] QuestionDto questionDto)
    {
        var existingQuestion = await databaseContext.Questions.FindAsync(questionId);
        if (existingQuestion == null) return NotFound();

        existingQuestion.QuestionText = questionDto.QuestionText;
        existingQuestion.QuestionTypeString = questionDto.QuestionType;
        existingQuestion.QuestionScore = questionDto.QuestionScore;
        existingQuestion.QuestionAnswer = questionDto.QuestionAnswer;
        existingQuestion.OptionsJson = questionDto.OptionsJson;
        existingQuestion.TestId = questionDto.TestId;

        await databaseContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{questionId}")]
    public async Task<IActionResult> Delete(int questionId)
    {
        var existingQuestion = await databaseContext.Questions.FindAsync(questionId);
        if (existingQuestion == null) return NotFound();

        databaseContext.Questions.Remove(existingQuestion);
        await databaseContext.SaveChangesAsync();
        return NoContent();
    }
}