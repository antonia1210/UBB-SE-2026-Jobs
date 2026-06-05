namespace UBB_SE_2026_Jobs.Api.Controllers;

    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Mappers;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _service;

        public LeaderboardController(ILeaderboardService service)
        {
            this._service = service;
        }

        [HttpGet("bytest/{testId}")]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> FindByTestId(int testId)
        {
            List<LeaderboardEntry> entries = await this._service.FindByTestIdAsync(testId);

            if (entries is null || !entries.Any())
                return NotFound($"No leaderboard entries found for test ID {testId}.");

            return Ok(entries.Select(entry => entry.ToDto()).ToList());
        }

        [HttpGet("bytest/{testId}/top/{limit}")]
        public async Task<ActionResult<List<LeaderboardEntryDto>>> FindTopByTestId(int testId, int limit)
        {
            List<LeaderboardEntry> entries = await this._service.FindTopByTestIdAsync(testId, limit);

            if (entries is null || !entries.Any())
                return NotFound($"No leaderboard entries found for test ID {testId}.");

            return Ok(entries.Select(entry => entry.ToDto()).ToList());
        }

        [HttpGet("bytest/{testId}/byuser/{userId}")]
        public async Task<ActionResult<LeaderboardEntryDto>> FindUserEntry(int userId, int testId)
        {
            LeaderboardEntry? entry = await this._service.FindUserEntryAsync(userId, testId);

            if (entry == null)
                return NotFound($"No leaderboard entry found for user ID {userId} and test ID {testId}.");

            return Ok(entry.ToDto());
        }

        [HttpDelete("bytest/{testId}")]
        public async Task<ActionResult> DeleteByTestId(int testId)
        {
            await this._service.DeleteByTestIdAsync(testId);

            return Ok(new { message = "Leaderboard entries deleted successfully" });
        }

        [HttpPost]
        public async Task<ActionResult> SaveRange([FromBody] List<LeaderboardEntryDto> dtos)
        {
            List<LeaderboardEntry> entries = dtos.Select(dto => dto.ToEntity()).ToList();
            await this._service.SaveRangeAsync(entries);

            return Ok();
        }

        [HttpPost("recalculate/{testId}")]
        public async Task<ActionResult> Recalculate(int testId)
        {
            await this._service.RecalculateAsync(testId);
            return Ok();
        }
    }


