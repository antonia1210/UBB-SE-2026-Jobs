// <copyright file="DataProcessingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Repositories.Skills;
    using UBB_SE_2026_Jobs.Library.Repositories.Users;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <inheritdoc cref="IDataProcessingService"/>
    public class DataProcessingService : IDataProcessingService
    {
        private readonly IUserRepository userRepository;
        private readonly ITestAttemptRepository attemptRepository;
        private readonly ITestRepository testRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IUserSkillRepository userSkillRepository;

        private const string CompletedStatus = "COMPLETED";
        private const decimal MinimumScore = 0m;


        public DataProcessingService(
            IUserRepository userRepository,
            ITestAttemptRepository attemptRepository,
            ITestRepository testRepository,
            ISkillRepository skillRepository,
            IUserSkillRepository userSkillRepository)
        {
            this.userRepository = userRepository;
            this.attemptRepository = attemptRepository;
            this.testRepository = testRepository;
            this.skillRepository = skillRepository;
            this.userSkillRepository = userSkillRepository;
        }

        /// <inheritdoc/>
        public async Task<bool> ProcessFinalizedAttemptAsync(int attemptId)
        {
            var attempt = await this.attemptRepository.FindByIdAsync(attemptId);

            if (attempt == null)
            {
                return false;
            }

            var validationError = await this.ValidateAttemptAsync(attempt);

            if (validationError != null)
            {
                attempt.IsValidated = false;
                attempt.PercentageScore = null;
                attempt.RejectionReason = validationError;
                attempt.RejectedAt = DateTime.UtcNow;

                await this.attemptRepository.UpdateAsync(attempt);
                return false;
            }

            float maxPossibleScore = attempt.Answers
                .Sum(a => a.Question?.QuestionScore ?? 0f);

            attempt.IsValidated = true;
            attempt.PercentageScore = maxPossibleScore > 0f
                ? (attempt.Score.GetValueOrDefault() / (decimal)maxPossibleScore) * 100m
                : 0m;
            attempt.RejectionReason = null;
            attempt.RejectedAt = null;

            await this.attemptRepository.UpdateAsync(attempt);

            if (attempt.IsValidated && attempt.PercentageScore.HasValue)
            {
                await this.UpdateUserSkillsFromAttemptAsync(attempt);
            }

            return true;
        }

        private async Task UpdateUserSkillsFromAttemptAsync(TestAttempt attempt)
        {
            if (attempt.ExternalUserId == null) return;

            var test = await this.testRepository.FindByIdAsync(attempt.TestId);
            if (test?.SkillId == null) return;

            var skill = test.Skill;
            if (skill == null)
            {
                var skills = await this.skillRepository.GetAllAsync();
                skill = skills.FirstOrDefault(s => s.SkillId == test.SkillId.Value);
            }

            if (skill == null) return;

            int userId = attempt.ExternalUserId.Value;
            var userSkill = await this.userSkillRepository.GetAsync(userId, skill.SkillId);

            int newScore = (int)Math.Round(attempt.PercentageScore.Value);

            if (userSkill == null)
            {
                var user = await this.userRepository.GetByIdAsync(userId);
                if (user == null) return;

                userSkill = new Domain.UserSkill
                {
                    Skill = skill,
                    User = user,
                    Score = newScore,
                    IsVerified = true,
                    AchievedDate = DateOnly.FromDateTime(DateTime.UtcNow)
                };
                await this.userSkillRepository.AddAsync(userSkill);
            }
            else if (newScore > userSkill.Score)
            {
                userSkill.Score = newScore;
                userSkill.IsVerified = true;
                userSkill.AchievedDate = DateOnly.FromDateTime(DateTime.UtcNow);
                await this.userSkillRepository.UpdateAsync(userSkill);
            }
        }

        /// <summary>
        /// Performs a series of business rule checks to ensure the attempt is eligible for processing.
        /// </summary>
        /// <param name="attempt">The <see cref="TestAttempt"/> entity to evaluate.</param>
        /// <returns>A string containing the rejection reason if invalid; otherwise, <c>null</c>.</returns>
        private async Task<string?> ValidateAttemptAsync(TestAttempt attempt)
        {
            if (attempt.ExternalUserId == null)
            {
                return "User does not exist.";
            }

            var user = await this.userRepository.GetByIdAsync(attempt.ExternalUserId.Value);
            if (user == null)
            {
                return "User does not exist.";
            }

            var test = await this.testRepository.FindByIdAsync(attempt.TestId);
            if (test == null)
            {
                return "Test does not exist.";
            }

            if (attempt.CompletedAt == null)
            {
                return "Attempt is incomplete. Missing completion time.";
            }

            if (string.IsNullOrWhiteSpace(attempt.Status))
            {
                return "Attempt status is missing.";
            }

            if (!string.Equals(attempt.Status, CompletedStatus, StringComparison.OrdinalIgnoreCase))
            {
                return "Attempt is not eligible for leaderboard because status is not COMPLETED.";
            }

            if (attempt.Score < MinimumScore)
            {
                return "Attempt score is invalid.";
            }

            return null;
        }

    }
}

