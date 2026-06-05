namespace UBB_SE_2026_Jobs.Library.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    /// <summary>
    /// ITestRepository interface provides methods to read static tests and questions.
    /// </summary>
    public interface ITestRepository
    {
        /// <summary>
        /// Asynchronously retrieves all test entities from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of all tests. The list is
        /// empty if no tests are found.</returns>

        public Task<List<Test>> GetTestsAsync();

        /// <summary>
        /// Asynchronously retrieves a list of all question category names associated with tests.
        /// </summary>
        /// <returns>A list of strings containing the names of all categories for questions that are linked to a test. The list
        /// will be empty if no such categories exist.</returns>
        public Task<List<string>> GetAllCategories();

        /// <summary>
        /// Finds a test by its ID, including its associated questions.
        /// </summary>
        /// <param name="id">The ID of the test to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<Test?> FindByIdAsync(int id);

        /// <summary>
        /// Asynchronously finds tests by their category, including their associated questions.
        /// </summary>
        /// <param name="category">The category of the tests to find.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<List<Test>> FindTestsByCategoryAsync(string category);

    }
}
