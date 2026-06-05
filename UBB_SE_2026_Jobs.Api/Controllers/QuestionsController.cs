using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBB_SE_2026_Jobs.Library.Persistence;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Api.Controllers;

    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;
        private readonly JobsDbContext _db;

        public QuestionsController(IQuestionService service, JobsDbContext db)
        {
            _service = service;
            _db = db;
        }

        [HttpGet("bytest/{testId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByTestId(int testId)
        {
            var questions = await _service.GetQuestionsByTestIdAsync(testId);
            if (questions == null || !questions.Any())
                return NotFound($"No questions found for test ID {testId}.");

            return Ok(questions.Select(q => q.ToDto()).ToList());
        }

        [HttpGet("byposition/{positionId}")]
        public async Task<ActionResult<List<QuestionDto>>> GetByPosition(int positionId)
        {
            var questions = await _service.GetInterviewQuestionsByPositionAsync(positionId);
            if (questions == null || !questions.Any())
                return NotFound($"No questions found for position ID {positionId}.");

            return Ok(questions.Select(q => q.ToDto()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> Get(int id)
        {
            var testQuestion = await _db.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
            if (testQuestion == null) return NotFound();
            return Ok(testQuestion.ToDto());
        }

    }

