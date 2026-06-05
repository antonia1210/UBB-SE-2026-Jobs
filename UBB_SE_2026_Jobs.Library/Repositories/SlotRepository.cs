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
        private readonly JobsDbContext databaseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlotRepository"/> class.
        /// </summary>
        public SlotRepository(JobsDbContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetSlotsAsync(int recruiterId, DateTime date)
        {
            return await this.databaseContext.Slots
                .Where(slot => slot.RecruiterId == recruiterId
                    && slot.StartTime.Date == date.Date)
                .OrderBy(slot => slot.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetAvailableByDateAsync(DateTime date)
        {
            return await this.databaseContext.Slots
                .Where(slot => slot.StartTime.Date == date.Date && slot.StatusValue == 0)
                .OrderBy(slot => slot.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetByCandidateAsync(int candidateId)
        {
            return await this.databaseContext.Slots
                .Where(slot => slot.CandidateId == candidateId && slot.StatusValue == 1)
                .OrderBy(slot => slot.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Slot>> GetAllSlotsAsync(int recruiterId)
        {
            return await this.databaseContext.Slots
                .Where(slot => slot.RecruiterId == recruiterId)
                .OrderBy(slot => slot.StartTime)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Slot?> GetByIdAsync(int slotId)
        {
            return await this.databaseContext.Slots
                .FirstOrDefaultAsync(slot => slot.Id == slotId);
        }

        /// <inheritdoc />
        public async Task AddAsync(Slot slot)
        {
            bool overlaps = await this.databaseContext.Slots
                .AnyAsync(existingSlot => existingSlot.RecruiterId == slot.RecruiterId
                    && existingSlot.StartTime.Date == slot.StartTime.Date
                    && slot.StartTime < existingSlot.EndTime
                    && slot.EndTime > existingSlot.StartTime);

            if (overlaps)
            {
                throw new InvalidOperationException("Slot overlaps with an existing appointment.");
            }

            this.databaseContext.Slots.Add(slot);
            await this.databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UpdateAsync(Slot slot)
        {
            bool overlaps = await this.databaseContext.Slots
                .AnyAsync(existingSlot => existingSlot.RecruiterId == slot.RecruiterId
                    && existingSlot.Id != slot.Id
                    && slot.StartTime < existingSlot.EndTime
                    && slot.EndTime > existingSlot.StartTime);

            if (overlaps)
            {
                throw new InvalidOperationException("Slot overlaps with an existing appointment.");
            }

            var existingSlot = await this.databaseContext.Slots.FindAsync(slot.Id);
            if (existingSlot == null)
            {
                throw new KeyNotFoundException("Slot not found.");
            }

            existingSlot.StartTime = slot.StartTime;
            existingSlot.EndTime = slot.EndTime;
            existingSlot.Duration = slot.Duration;
            await this.databaseContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAsync(int slotId)
        {
            var slot = await this.databaseContext.Slots.FindAsync(slotId);
            if (slot != null)
            {
                this.databaseContext.Slots.Remove(slot);
                await this.databaseContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public List<Slot> GetSlots(int recruiterId, DateTime date)
        {
            return this.databaseContext.Slots
                .Where(slot => slot.RecruiterId == recruiterId
                    && slot.StartTime.Date == date.Date)
                .OrderBy(slot => slot.StartTime)
                .ToList();
        }

        /// <inheritdoc />
        public List<Slot> GetAllSlots(int recruiterId)
        {
            return this.databaseContext.Slots
                .Where(slot => slot.RecruiterId == recruiterId)
                .OrderBy(slot => slot.StartTime)
                .ToList();
        }

        /// <inheritdoc />
        public Slot? GetById(int slotId)
        {
            return this.databaseContext.Slots
                .FirstOrDefault(slot => slot.Id == slotId);
        }

        /// <inheritdoc />
        public void Add(Slot slot)
        {
            bool overlaps = this.databaseContext.Slots
                .Any(existingSlot => existingSlot.RecruiterId == slot.RecruiterId
                    && existingSlot.StartTime.Date == slot.StartTime.Date
                    && slot.StartTime < existingSlot.EndTime
                    && slot.EndTime > existingSlot.StartTime);

            if (overlaps)
            {
                throw new InvalidOperationException("Slot overlaps with an existing appointment.");
            }

            this.databaseContext.Slots.Add(slot);
            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc />
        public void Update(Slot slot)
        {
            var existingSlot = this.databaseContext.Slots.Find(slot.Id);
            if (existingSlot == null)
            {
                throw new KeyNotFoundException("Slot not found.");
            }

            existingSlot.StartTime = slot.StartTime;
            existingSlot.EndTime = slot.EndTime;
            existingSlot.RecruiterId = slot.RecruiterId;
            existingSlot.Duration = slot.Duration;
            existingSlot.StatusValue = slot.StatusValue;
            existingSlot.InterviewType = slot.InterviewType;

            this.databaseContext.SaveChanges();
        }

        /// <inheritdoc />
        public void Delete(int slotId)
        {
            var slot = this.databaseContext.Slots.Find(slotId);
            if (slot != null)
            {
                this.databaseContext.Slots.Remove(slot);
                this.databaseContext.SaveChanges();
            }
        }
    }
}