using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Services.Recommendations;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendations;
using UBB_SE_2026_Jobs.Library.Services.UserRecommendationService;

namespace UBB_SE_2026_Jobs.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/recommendations")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService recommendationService;
    private readonly IUserRecommendationService userRecommendationService;

    public RecommendationsController(IRecommendationService recommendationService, IUserRecommendationService userRecommendationService)
    {
        this.recommendationService = recommendationService;
        this.userRecommendationService = userRecommendationService;
    }

    [HttpGet("{recommendationId}")]
    public async Task<IActionResult> GetById(int recommendationId, CancellationToken cancellationToken)
    {
        var recommendation = await recommendationService.GetByIdAsync(recommendationId, cancellationToken);
        return recommendation is null ? NotFound() : Ok(recommendation);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? userId, [FromQuery] int? jobId, CancellationToken cancellationToken)
    {
        if (userId.HasValue && jobId.HasValue)
        {
            var recommendation = await recommendationService.GetLatestForUserAndJobAsync(userId.Value, jobId.Value, cancellationToken);
            return Ok(recommendation);
        }

        return Ok(await recommendationService.GetAllAsync(cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateRecommendationRequest createRecommendationRequest, CancellationToken cancellationToken)
    {
        try
        {
            DateTime? timestamp = createRecommendationRequest.Timestamp == default ? null : createRecommendationRequest.Timestamp;
            var savedRecommendation = await recommendationService.AddAsync(createRecommendationRequest.UserId, createRecommendationRequest.JobId, timestamp, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { recommendationId = savedRecommendation.RecommendationId }, savedRecommendation);
        }
        catch (KeyNotFoundException keyNotFoundException)
        {
            return NotFound(keyNotFoundException.Message);
        }
    }

    [HttpPut("{recommendationId}")]
    public async Task<IActionResult> Update(int recommendationId, [FromBody] UpdateRecommendationRequest updateRecommendationRequest, CancellationToken cancellationToken)
    {
        try
        {
            await recommendationService.UpdateTimestampAsync(recommendationId, updateRecommendationRequest.Timestamp, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{recommendationId}")]
    public async Task<IActionResult> Remove(int recommendationId, CancellationToken cancellationToken)
    {
        if (await recommendationService.GetByIdAsync(recommendationId, cancellationToken) is null)
            return NotFound();

        await recommendationService.RemoveAsync(recommendationId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{userId}/next")]
    public async Task<IActionResult> GetNextCard(int userId, [FromBody] UserMatchmakingFilters filters, CancellationToken cancellationToken)
    {
        var card = await userRecommendationService.GetNextCardAsync(userId, filters, cancellationToken);
        if (card is null)
        {
            return NoContent();
        }
        return Ok(card);
    }

    [HttpPost("{userId}/fallback")]
    public async Task<IActionResult> GetFallbackCard(int userId, [FromBody] UserMatchmakingFilters filters, CancellationToken cancellationToken)
    {
        var card = await userRecommendationService.RecalculateTopCardIgnoringCooldownAsync(userId, filters, cancellationToken);
        if (card is null)
        {
            return NoContent();
        }
        return Ok(card);
    }

    [HttpPost("{userId}/like")]
    public async Task<IActionResult> ApplyLike(int userId, [FromBody] JobRecommendationResult card, CancellationToken cancellationToken)
    {
        try
        {
            int matchId = await userRecommendationService.ApplyLikeAsync(userId, card, cancellationToken);
            return Ok(matchId);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            return BadRequest(invalidOperationException.Message);
        }
    }

    [HttpPost("{userId}/dismiss")]
    public async Task<IActionResult> ApplyDismiss(int userId, [FromBody] JobRecommendationResult card, CancellationToken cancellationToken)
    {
        int dismissRecommendationId = await userRecommendationService.ApplyDismissAsync(userId, card, cancellationToken);
        return Ok(dismissRecommendationId);
    }

    [HttpPost("undo-like")]
    public async Task<IActionResult> UndoLike([FromQuery] int matchId, [FromQuery] int? displayId, CancellationToken cancellationToken)
    {
        await userRecommendationService.UndoLikeAsync(matchId, displayId, cancellationToken);
        return NoContent();
    }

    [HttpPost("undo-dismiss")]
    public async Task<IActionResult> UndoDismiss([FromQuery] int dismissId, [FromQuery] int? displayId, CancellationToken cancellationToken)
    {
        await userRecommendationService.UndoDismissAsync(dismissId, displayId, cancellationToken);
        return NoContent();
    }

    public record CreateRecommendationRequest(int UserId, int JobId, DateTime Timestamp);
    public record UpdateRecommendationRequest(DateTime Timestamp);
}