namespace UBB_SE_2026_Jobs.Library.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using UBB_SE_2026_Jobs.Library.Persistence;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Domain.Enums;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;

    /// <summary>
    /// Provides methods for managing Slot entities in the database, including retrieval, creation, update, and deletion
    /// operations for recruiter slots.
    /// </summary>
    public class SlotRepository : ISlotRepository
    {
        private readonly JobsDbContext JobsDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotRepository"/> class.
        /// </summary>
        public SlotRepository(JobsDbContext JobsDbContext)
        {
            this.JobsDbContext = JobsDbContext;
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetSlotsAsync(int recruiterId, DateTime date)
        {
            return await this.JobsDbContext.Slots
                .Where(s => s.RecruiterId == recruiterId
                    && s.StartTime.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetAvailableByDateAsync(DateTime date)
        {
            return await this.JobsDbContext.Slots
                .Where(s => s.StartTime.Date == date.Date && s.StatusValue == 0)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetByCandidateAsync(int candidateId)
        {
            return await this.JobsDbContext.Slots
                .Where(s => s.CandidateId == candidateId && s.StatusValue == 1)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetAllSlotsAsync(int recruiterId)
        {
            return await this.JobsDbContext.Slots
                .Where(s => s.RecruiterId == recruiterId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Slot?> GetByIdAsync(int id)
        {
            return await this.JobsDbContext.Slots
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <inheritdoc />
        public async Task AddAsync(Slot slot)
        {
            bool overlaps = await this.JobsDbContext.Slots
                .AnyAsync(s => s.RecruiterId == slot.RecruiterId
                    && s.StartTime.Date == slot.StartTime.Date
                    && slot.StartTime < s.EndTime
                    && slot.EndTime > s.StartTime);

            if (overlaps)
            {
                throw new Exception("Slot overlaps with an existing appointment!");
            }

            this.JobsDbContext.Slots.Add(slot);
            await this.JobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Slot slot)
        {
            bool overlaps = await this.JobsDbContext.Slots
                .AnyAsync(s => s.RecruiterId == slot.RecruiterId
                    && s.Id != slot.Id
                    && slot.StartTime < s.EndTime
                    && slot.EndTime > s.StartTime);

            if (overlaps)
            {
                throw new Exception("Slot overlaps with an existing appointment!");
            }

            var existing = await this.JobsDbContext.Slots.FindAsync(slot.Id);
            if (existing == null)
            {
                throw new Exception("Slot not found");
            }

            existing.StartTime = slot.StartTime;
            existing.EndTime = slot.EndTime;
            existing.Duration = slot.Duration;
            await this.JobsDbContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int id)
        {
            var slot = await this.JobsDbContext.Slots.FindAsync(id);
            if (slot != null)
            {
                this.JobsDbContext.Slots.Remove(slot);
                await this.JobsDbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public List<Slot> GetSlots(int recruiterId, DateTime date)
        {
            return this.JobsDbContext.Slots
                .Where(s => s.RecruiterId == recruiterId
                    && s.StartTime.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        /// <inheritdoc />
        public List<Slot> GetAllSlots(int recruiterId)
        {
            return this.JobsDbContext.Slots
                .Where(s => s.RecruiterId == recruiterId)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        /// <inheritdoc />
        public Slot? GetById(int id)
        {
            return this.JobsDbContext.Slots
                .FirstOrDefault(s => s.Id == id);
        }

        /// <inheritdoc />
        public void Add(Slot slot)
        {
            bool overlaps = this.JobsDbContext.Slots
                .Any(s => s.RecruiterId == slot.RecruiterId
                    && s.StartTime.Date == slot.StartTime.Date
                    && slot.StartTime < s.EndTime
                    && slot.EndTime > s.StartTime);

            if (overlaps)
            {
                throw new Exception("Slot overlaps with an existing appointment!");
            }

            this.JobsDbContext.Slots.Add(slot);
            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc />
        public void Update(Slot slot)
        {
            var existing = this.JobsDbContext.Slots.Find(slot.Id);
            if (existing == null)
            {
                throw new Exception("Slot not found");
            }

            existing.StartTime = slot.StartTime;
            existing.EndTime = slot.EndTime;
            existing.RecruiterId = slot.RecruiterId;
            existing.Duration = slot.Duration;
            existing.StatusValue = slot.StatusValue;
            existing.InterviewType = slot.InterviewType;

            this.JobsDbContext.SaveChanges();
        }

        /// <inheritdoc />
        public void Delete(int id)
        {
            var slot = this.JobsDbContext.Slots.Find(id);
            if (slot != null)
            {
                this.JobsDbContext.Slots.Remove(slot);
                this.JobsDbContext.SaveChanges();
            }
        }
    }
}

