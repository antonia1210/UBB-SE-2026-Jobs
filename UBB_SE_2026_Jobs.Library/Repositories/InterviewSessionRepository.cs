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
        private readonly JobsDbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterviewSessionRepository"/> class.
        /// </summary>
        public InterviewSessionRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc/>
        public async Task<InterviewSession> GetInterviewSessionByIdAsync(int sessionId)
        {
            var interviewSession = await this.databaseContext.InterviewSessions
                .FirstOrDefaultAsync(interviewSession => interviewSession.Id == sessionId);

            if (interviewSession == null)
            {
                throw new KeyNotFoundException($"InterviewSession with ID {sessionId} was not found.");
            }

            return interviewSession;
        }

        /// <inheritdoc/>
        public InterviewSession GetInterviewSessionById(int sessionId)
        {
            var interviewSession = this.databaseContext.InterviewSessions
                .FirstOrDefault(interviewSession => interviewSession.Id == sessionId);

            if (interviewSession == null)
            {
                throw new KeyNotFoundException($"InterviewSession with ID {sessionId} was not found.");
            }

            return interviewSession;
        }

        /// <inheritdoc/>
        public async Task UpdateInterviewSessionAsync(InterviewSession updatedInterviewSession)
        {
            var existingInterviewSession = await this.databaseContext.InterviewSessions
                .FirstOrDefaultAsync(interviewSession => interviewSession.Id == updatedInterviewSession.Id);

            if (existingInterviewSession == null)
            {
                return;
            }

            existingInterviewSession.InterviewerId = updatedInterviewSession.InterviewerId;
            existingInterviewSession.PositionId = updatedInterviewSession.PositionId;
            existingInterviewSession.ExternalUserId = updatedInterviewSession.ExternalUserId;
            existingInterviewSession.Status = updatedInterviewSession.Status;
            existingInterviewSession.DateStart = updatedInterviewSession.DateStart;
            existingInterviewSession.Video = updatedInterviewSession.Video;
            existingInterviewSession.Score = updatedInterviewSession.Score;

            await this.databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public void Add(InterviewSession interviewSession)
        {
            this.databaseContext.InterviewSessions.Add(interviewSession);
            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc/>
        public void Delete(InterviewSession interviewSession)
        {
            this.databaseContext.InterviewSessions.Remove(interviewSession);
            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetScheduledSessionsAsync()
        {
            return await this.databaseContext.InterviewSessions
                .Where(interviewSession => interviewSession.Status == InterviewStatus.Scheduled.ToString())
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<List<InterviewSession>> GetSessionsByStatusAsync(string status)
        {
            return await this.databaseContext.InterviewSessions
                .Where(interviewSession => interviewSession.Status == status)
                .OrderByDescending(interviewSession => interviewSession.DateStart)
                .ToListAsync();
        }
    }
}