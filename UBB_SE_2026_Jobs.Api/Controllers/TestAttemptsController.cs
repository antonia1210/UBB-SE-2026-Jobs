using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestAttemptsController : ControllerBase
{
    private readonly ITestAttemptService testAttemptService;

    public TestAttemptsController(ITestAttemptService testAttemptService)
    {
        this.testAttemptService = testAttemptService;
    }

    [HttpGet("{attemptId}")]
    public async Task<ActionResult<TestAttemptDto>> FindById(int attemptId)
    {
        TestAttempt? testAttempt = await this.testAttemptService.FindByIdAsync(attemptId);

        if (testAttempt == null)
        {
            return NotFound();
        }

        return Ok(testAttempt.ToDto());
    }

    [HttpGet("byuser/{userId}/bytest/{testId}")]
    public async Task<ActionResult<TestAttemptDto>> FindByUserAndTest(int userId, int testId)
    {
        TestAttempt? testAttempt = await this.testAttemptService.FindByUserAndTestAsync(userId, testId);

        if (testAttempt == null)
        {
            return NotFound();
        }

        return Ok(testAttempt.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> Save([FromBody] TestAttemptDto testAttemptDto)
    {
        await this.testAttemptService.SaveAsync(testAttemptDto.ToEntity());
        return Ok();
    }

    [HttpPut("{attemptId}")]
    public async Task<ActionResult<TestAttemptDto>> Update(int attemptId, [FromBody] TestAttemptDto testAttemptDto)
    {
        TestAttempt testAttempt = testAttemptDto.ToEntity();
        testAttempt.Id = attemptId;
        TestAttempt? updatedTestAttempt = await this.testAttemptService.UpdateAsync(testAttempt);

        if (updatedTestAttempt == null)
        {
            return NotFound();
        }

        return Ok(updatedTestAttempt.ToDto());
    }

    [HttpGet("valid/bytest/{testId}")]
    public async Task<ActionResult<List<TestAttemptDto>>> FindValidAttemptsByTestId(int testId)
    {
        List<TestAttempt> testAttempts = await this.testAttemptService.FindValidAttemptsByTestIdAsync(testId);
        return Ok(testAttempts.Select(testAttempt => testAttempt.ToDto()).ToList());
    }

    [HttpGet("byuser/{userId}")]
    public async Task<ActionResult<List<TestAttemptDto>>> FindByUserId(int userId)
    {
        var testAttempts = await this.testAttemptService.FindByUserId(userId);
        return Ok(testAttempts.Select(testAttempt => testAttempt.ToDto()).ToList());
    }
}