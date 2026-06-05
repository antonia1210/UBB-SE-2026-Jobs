// <copyright file="TestsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Mappers;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.Services.Interfaces;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestsController : ControllerBase
{
    private readonly ITestService testService;

    public TestsController(ITestService testService)
    {
        this.testService = testService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TestDto>>> GetAll()
    {
        List<Test> tests = await this.testService.GetAll();
        return Ok(tests.Select(test => test.ToDto()).ToList());
    }

    [HttpGet("categories")]
    public async Task<ActionResult<string>> GetCategories()
    {
        return Ok(await this.testService.GetCategories());
    }

    [HttpGet("{testId}")]
    public async Task<ActionResult<TestDto>> FindById(int testId)
    {
        Test? test = await this.testService.FindByIdAsync(testId);

        if (test == null)
        {
            return NotFound();
        }

        return Ok(test.ToDto());
    }

    [HttpGet("bycategory/{category}")]
    public async Task<ActionResult<List<TestDto>>> FindByCategory(string category)
    {
        List<Test> tests = await this.testService.FindTestsByCategoryAsync(category);
        return Ok(tests.Select(test => test.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<TestDto>> Create([FromBody] TestDto testDto)
    {
        try
        {
            Test createdTest = await this.testService.AddTestASync(testDto.ToEntity());
            return Ok(createdTest.ToDto());
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{testId}")]
    public async Task<ActionResult<TestDto>> Update(int testId, [FromBody] TestDto testDto)
    {
        try
        {
            Test updatedTest = await this.testService.UpdateTestAsync(testId, testDto.ToEntity());
            return Ok(updatedTest.ToDto());
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
        catch (Exception exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{testId}")]
    public async Task<ActionResult> Delete(int testId)
    {
        try
        {
            bool deleted = await this.testService.DeleteTestAsync(testId);

            if (deleted)
            {
                return Ok(new { message = "Test was deleted successfully" });
            }

            return BadRequest();
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
    }

    /// <summary>
    /// Starts a test attempt for the specified user and test.
    /// </summary>
    [HttpPost("start")]
    public async Task<ActionResult> StartTest([FromBody] StartTestDto startTestDto)
    {
        try
        {
            await this.testService.StartTestAsync(startTestDto.UserId, startTestDto.TestId);
            return this.Ok();
        }
        catch (InvalidOperationException invalidOperationException)
        {
            return this.Conflict(invalidOperationException.Message);
        }
    }

    /// <summary>
    /// Submits a test attempt with answers and returns the final score.
    /// </summary>
    [HttpPost("submit-attempt")]
    public async Task<ActionResult<float>> SubmitAttempt([FromBody] SubmitAttemptDto submitAttemptDto)
    {
        float score = await this.testService.SubmitAttemptAsync(
            submitAttemptDto.UserId, submitAttemptDto.TestId, submitAttemptDto.Answers);
        return this.Ok(score);
    }
}