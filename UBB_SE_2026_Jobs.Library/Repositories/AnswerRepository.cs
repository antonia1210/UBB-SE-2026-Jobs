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
    /// AnswerRepository class provides methods to perform CRUD operations on the Answers table in the database.
    /// </summary>
    public class AnswerRepository : IAnswerRepository
    {
        private readonly JobsDbContext JobsDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerRepository"/> class.
        /// </summary>
        public AnswerRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Answer answer)
        {
            this.JobsDbContext.Answers.Add(answer);
            await this.JobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Answer>> FindByAttemptAsync(int attemptId)
        {
            return await this.JobsDbContext.Answers
                .Include(a => a.Question)
                .Where(a => a.AttemptId == attemptId)
                .ToListAsync();
        }
    }
}

