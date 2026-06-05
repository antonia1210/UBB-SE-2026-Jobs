namespace UBB_SE_2026_Jobs.Library.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Provides operations for retrieving questions by test or position.
    /// </summary>
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access question data. Cannot be null.</param>
        public QuestionService(IQuestionRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously retrieves all questions belonging to the specified test.
        /// </summary>
        /// <param name="testId">The unique identifier of the test.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of questions for the specified test.</returns>
        public async Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            return await this._repository.FindByTestIdAsync(testId);
        }

        /// <summary>
        /// Asynchronously retrieves all interview questions for the specified position.
        /// </summary>
        /// <param name="positionId">The unique identifier of the position.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of interview questions for the specified position.</returns>
        public async Task<List<TestQuestion>> GetInterviewQuestionsByPositionAsync(int positionId)
        {
            return await this._repository.GetInterviewQuestionsByPositionAsync(positionId);
        }

        public async Task<TestQuestion?> GetByIdAsync(int questionId)
        {
            return await this._repository.FindByIdWithAnswersAsync(questionId);
        }
    }
}
