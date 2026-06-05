// <copyright file="ITestService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    /// <summary>
    /// Defines operations for managing tests.
    /// </summary>
    public interface ITestService
    {
        /// <summary>
        /// Asynchronously retrieves all test entities from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all test entities. The
        /// list will be empty if no tests are found.</returns>
        public Task<List<Test>> GetAll();

        /// <summary>
        /// Asynchronously retrieves a list of all available category names.
        /// </summary>
        /// <returns>A list of strings containing the names of all categories. The list is empty if no categories are found.</returns>
        public Task<List<string>> GetCategories();

        /// <summary>
        /// Asynchronously retrieves the test with the specified identifier, including its associated questions.
        /// </summary>
        /// <param name="id">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the test, or null if not found.</returns>
        Task<Test?> FindByIdAsync(int id);

        /// <summary>
        /// Asynchronously retrieves all tests belonging to the specified category, including their associated questions.
        /// </summary>
        /// <param name="category">The category to filter tests by.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of tests in the specified category.</returns>
        Task<List<Test>> FindTestsByCategoryAsync(string category);

        /// <summary>
        /// Starts a test attempt for the specified user and test.
        /// </summary>
        Task StartTestAsync(int userId, int testId);

        /// <summary>
        /// Submits a test attempt by grading all answers and calculating the final score.
        /// </summary>
        Task SubmitTestAsync(int attemptId);

        /// <summary>
        /// Submits a full attempt with answers, grades it, and returns the final score.
        /// </summary>
        Task<float> SubmitAttemptAsync(int userId, int testId, IEnumerable<AnswerDto> answers);
    }
}
