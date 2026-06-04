namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Domain.Portal.Enums;
    using UBB_SE_2026_Jobs.Library.Repositories.Portal;

    /// <summary>
    /// QuestionRepository class provides methods to perform CRUD operations on the Questions table in the database.
    /// </summary>
    public class QuestionRepository : IQuestionRepository
    {
        private readonly PortalDbContext appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepository"/> class.
        /// </summary>
        public QuestionRepository(PortalDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc />
        public async Task<List<Question>> FindByTestIdAsync(int testId)
        {
            return await this.appDbContext.Questions
                .Include(q => q.Answers)
                .Where(q => q.TestId == testId)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Question>> GetInterviewQuestionsByPositionAsync(int positionId)
        {
            return await this.appDbContext.Questions
                .Where(q => q.QuestionTypeString == QuestionType.INTERVIEW.ToString()
                    && q.PositionId == positionId)
                .ToListAsync();
        }
    }
}