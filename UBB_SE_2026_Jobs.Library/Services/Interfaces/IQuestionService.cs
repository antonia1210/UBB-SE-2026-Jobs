namespace UBB_SE_2026_Jobs.Library.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Core;

    /// <summary>
    /// Defines operations for managing questions.
    /// </summary>
    public interface IQuestionService
    {
        /// <summary>
        /// Asynchronously retrieves all questions belonging to the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions for the specified test.</returns>
        Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId);

        /// <summary>
        /// Asynchronously retrieves all interview questions for the specified position.
        /// </summary>
        /// <param name="positionId">The unique identifier of the position.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of interview questions for the specified position.</returns>
        Task<List<TestQuestion>> GetInterviewQuestionsByPositionAsync(int positionId);
    }
}
