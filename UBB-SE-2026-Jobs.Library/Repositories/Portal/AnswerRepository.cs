namespace UBB_SE_2026_Jobs.Library.Repositories.Portal
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;
    using UBB_SE_2026_Jobs.Library.Repositories.Portal;

    /// <summary>
    /// AnswerRepository class provides methods to perform CRUD operations on the Answers table in the database.
    /// </summary>
    public class AnswerRepository : IAnswerRepository
    {
        private readonly PortalDbContext appDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerRepository"/> class.
        /// </summary>
        public AnswerRepository(PortalDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        /// <inheritdoc/>
        public async Task SaveAsync(Answer answer)
        {
            this.appDbContext.Answers.Add(answer);
            await this.appDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<Answer>> FindByAttemptAsync(int attemptId)
        {
            return await this.appDbContext.Answers
                .Include(a => a.Question)
                .Where(a => a.AttemptId == attemptId)
                .ToListAsync();
        }
    }
}