namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Domain.Enums;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <summary>
    /// Repository for managing interview sessions.
    /// </summary>
    public class InterviewSessionRepository : IInterviewSessionRepository
    {
        private readonly JobsDbContext JobsDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterviewSessionRepository"/> class.
        /// </summary>
        public InterviewSessionRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc/>
        public async Task<InterviewSession> GetInterviewSessionByIdAsync(int id)
        {
            var session = await this.JobsDbContext.InterviewSessions
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
            {
                throw new KeyNotFoundException($"InterviewSession with ID {id} was not found.");
            }

            return session;
        }

        /// <inheritdoc/>
        public InterviewSession GetInterviewSessionById(int id)
        {
            var session = this.JobsDbContext.InterviewSessions
                .FirstOrDefault(s => s.Id == id);

            if (session == null)
            {
                throw new KeyNotFoundException($"InterviewSession with ID {id} was not found.");
            }

            return session;
        }

        /// <inheritdoc/>
        public async Task UpdateInterviewSessionAsync(InterviewSession updated)
        {
            var existing = await this.JobsDbContext.InterviewSessions
                .FirstOrDefaultAsync(s => s.Id == updated.Id);

            if (existing == null)
            {
                return;
            }

            existing.InterviewerId = updated.InterviewerId;
            existing.PositionId = updated.PositionId;
            existing.ExternalUserId = updated.ExternalUserId;
            existing.Status = updated.Status;
            existing.DateStart = updated.DateStart;
            existing.Video = updated.Video;
            existing.Score = updated.Score;

            await this.JobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void Add(InterviewSession session)
        {
            this.JobsDbContext.InterviewSessions.Add(session);
            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void Delete(InterviewSession session)
        {
            this.JobsDbContext.InterviewSessions.Remove(session);
            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetScheduledSessionsAsync()
        {
            return await this.JobsDbContext.InterviewSessions
                .Where(s => s.Status == InterviewStatus.Scheduled.ToString())
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
        {
            return await this.JobsDbContext.InterviewSessions
                .Where(s => s.Status == status)
                .OrderByDescending(s => s.DateStart)
                .ToListAsync();
        }
    }
}

