namespace UBB_SE_2026_Jobs.Library.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    /// <summary>
    /// ITestAttemptRepository interface provides methods to perform CRUD operations on the TestAttempts.
    /// </summary>
    public interface ITestAttemptRepository
    {
        /// <summary>
        /// Finds a test attempt by its ID, including answers and questions.
        /// </summary>
        Task<TestAttempt?> FindByIdAsync(int id);

        /// <summary>
        /// Finds a test attempt by user ID and test ID, including answers.
        /// </summary>
        Task<TestAttempt?> FindByUserAndTestAsync(int userId, int testId);

        /// <summary>
        /// Saves a new test attempt.
        /// </summary>
        Task SaveAsync(TestAttempt attempt);

        /// <summary>
        /// Updates an existing test attempt. Returns null if not found.
        /// </summary>
        Task<TestAttempt?> UpdateAsync(TestAttempt attempt);

        /// <summary>
        /// Returns all valid (COMPLETED + validated) attempts for a given test,
        /// ordered by score descending then completion time ascending.
        /// Used for leaderboard calculations.
        /// </summary>
        Task<List<TestAttempt>> FindValidAttemptsByTestIdAsync(int testId);

        /// <summary>
        /// Returns all completed, validated attempts for a given user,
        /// including the parent Test so callers can read Test.Title.
        /// Replaces the former SkillTestRepository.GetByUserIdAsync.
        /// </summary>
        Task<IReadOnlyList<TestAttempt>> FindCompletedByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
