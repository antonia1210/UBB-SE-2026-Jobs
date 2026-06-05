// <copyright file="TestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <summary>
    /// TestRepository class provides methods to perform CRUD operations on the Tests and Questions tables in the database.
    /// </summary>
    public class TestRepository : ITestRepository
    {
        private readonly JobsDbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRepository"/> class.
        /// </summary>
        public TestRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <summary>
        /// Asynchronously retrieves all test entities from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all tests. The list is
        /// empty if no tests are found.</returns>
        public async Task<List<Test>> GetTestsASync()
        {
            return await this.databaseContext.Tests.ToListAsync();
        }

        /// <summary>
        /// Asynchronously retrieves a list of all question category names associated with tests.
        /// </summary>
        /// <returns>A list of strings containing the names of all categories for questions that are linked to a test. The list
        /// will be empty if no such categories exist.</returns>
        public async Task<List<string>> GetAllCategories()
        {
            return await this.databaseContext.Tests
                .Select(test => test.Category)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously finds a test by its identifier, including its associated questions.
        /// </summary>
        /// <param name="testId">The identifier of the test to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<Test?> FindByIdAsync(int testId)
        {
            return await this.databaseContext.Tests
                .Include(test => test.Questions)
                .FirstOrDefaultAsync(test => test.Id == testId);
        }

        /// <summary>
        /// Asynchronously finds tests by their category, including their associated questions.
        /// </summary>
        /// <param name="category">The category of the tests to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<List<Test>> FindTestsByCategoryAsync(string category)
        {
            return await this.databaseContext.Tests
                .Include(test => test.Questions)
                .Where(test => test.Category == category)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously adds a new Test entity to the data store.
        /// </summary>
        /// <param name="test">The Test entity to add. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public async Task AddAsync(Test test)
        {
            this.databaseContext.Tests.Add(test);
            await this.databaseContext.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously updates the specified test entity in the database.
        /// </summary>
        /// <param name="test">The test entity containing updated values. The entity's identifier must correspond to an existing test in the
        /// database.</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a test with the specified identifier does not exist in the database.</exception>
        public async Task UpdateAsync(Test test)
        {
            var existingTest = await this.databaseContext.Tests.FindAsync(test.Id);
            if (existingTest == null)
            {
                throw new KeyNotFoundException("Test not found.");
            }

            existingTest.Title = test.Title;
            existingTest.Category = test.Category;
            await this.databaseContext.SaveChangesAsync();
        }

        /// <summary>
        /// Asynchronously deletes the test entity with the specified identifier from the data store.
        /// </summary>
        /// <param name="testId">The unique identifier of the test entity to delete.</param>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if a test entity with the specified identifier does not exist.</exception>
        public async Task DeleteAsync(int testId)
        {
            var existingTest = await this.databaseContext.Tests.FindAsync(testId);
            if (existingTest == null)
            {
                throw new KeyNotFoundException("Test not found.");
            }

            this.databaseContext.Tests.Remove(existingTest);
            await this.databaseContext.SaveChangesAsync();
        }
    }
}