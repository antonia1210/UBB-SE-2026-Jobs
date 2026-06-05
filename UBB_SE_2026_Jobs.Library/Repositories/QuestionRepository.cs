namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Domain.Enums;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <summary>
    /// QuestionRepository class provides methods to perform CRUD operations on the Questions table in the database.
    /// </summary>
    public class QuestionRepository : IQuestionRepository
    {
        private readonly JobsDbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepository"/> class.
        /// </summary>
        public QuestionRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<List<TestQuestion>> FindByTestIdAsync(int testId)
        {
            return await this.databaseContext.Questions
                .Include(question => question.Answers)
                .Where(question => question.TestId == testId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<TestQuestion>> GetInterviewQuestionsByPositionAsync(int positionId)
        {
            return await this.databaseContext.Questions
                .Where(question => question.QuestionTypeString == QuestionType.INTERVIEW.ToString()
                    && question.PositionId == positionId)
                .ToListAsync();
        }
    }
}