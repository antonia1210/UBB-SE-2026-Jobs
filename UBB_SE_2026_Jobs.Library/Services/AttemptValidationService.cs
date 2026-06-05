// <copyright file="AttemptValidationService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Services
{
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// AttemptValidationService class provides methods to validate if a user can start a test attempt and to check for existing attempts.
    /// </summary>
    public class AttemptValidationService : IAttemptValidationService
    {
        private readonly ITestAttemptRepository attemptRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptValidationService"/> class.
        /// </summary>
        /// <param name="attemptRepository">The repository used to access test attempt data.</param>
        public AttemptValidationService(ITestAttemptRepository attemptRepository)
        {
            this.attemptRepository = attemptRepository;
        }

        /// <summary>
        /// Asynchronously checks if a user can start a test attempt by verifying if there are
        /// any existing attempts for the given user and test.
        /// </summary>
        /// <param name="userId">The ID of the user attempting to start the test.</param>
        /// <param name="testId">The ID of the test the user is attempting to start.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<bool> CanStartTestAsync(int userId, int testId)
        {
            // Tests are replayable — only block if an IN_PROGRESS attempt exists.
            // FindByUserAndTestAsync returns the active IN_PROGRESS attempt or null.
            var active = await this.attemptRepository.FindByUserAndTestAsync(userId, testId);
            return active == null;
        }

        /// <summary>
        /// Throws only when an IN_PROGRESS attempt already exists for the user/test.
        /// Completed attempts are allowed — tests are replayable.
        /// </summary>
        public async Task CheckExistingAttemptsAsync(int userId, int testId)
        {
            var active = await this.attemptRepository.FindByUserAndTestAsync(userId, testId);
            if (active != null)
            {
                throw new System.InvalidOperationException(
                    $"User {userId} already has an active in-progress attempt for test {testId}.");
            }
        }
    }
}
