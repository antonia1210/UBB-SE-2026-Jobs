namespace UBB_SE_2026_Jobs.Library.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Provides operations for managing answers.
    /// </summary>
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerService"/> class.
        /// </summary>
        /// <param name="repository">The repository used to access answer data. Cannot be null.</param>
        public AnswerService(IAnswerRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Asynchronously saves the specified answer.
        /// </summary>
        /// <param name="answer">The answer to save. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SaveAsync(Answer answer)
        {
            await this._repository.SaveAsync(answer);
        }

        /// <summary>
        /// Asynchronously retrieves all answers associated with the specified attempt.
        /// </summary>
        /// <param name="attemptId">The unique identifier of the attempt.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of answers for the specified attempt.</returns>
        public async Task<List<Answer>> FindByAttemptAsync(int attemptId)
        {
            return await this._repository.FindByAttemptAsync(attemptId);
        }
    }
}
