// <copyright file="BookingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UBB_SE_2026_Jobs.Library.Domain;
    using UBB_SE_2026_Jobs.Library.Domain.Core;
    using UBB_SE_2026_Jobs.Library.Domain.Enums;
    using UBB_SE_2026_Jobs.Library.Repositories;
    using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
    using UBB_SE_2026_Jobs.Library.Repositories.Matches;
    using UBB_SE_2026_Jobs.Library.Services.Interfaces;

    /// <summary>
    /// Provides booking-related operations for managing interview slot reservations and session creation.
    /// </summary>
    public class BookingService : IBookingService
    {
        private const int MINIMUMPOSITIONID = 0;
        private const int MINIMUMINTERVIEWSCORE = 0;

        private readonly ISlotRepository _slotRepository;
        private readonly IInterviewSessionRepository _interviewSessionRepository;
        private readonly IMatchRepository _matchRepository;

        public BookingService(ISlotRepository slotRepository, IInterviewSessionRepository interviewSessionRepository, IMatchRepository matchRepository)
        {
            this._slotRepository = slotRepository;
            this._interviewSessionRepository = interviewSessionRepository;
            this._matchRepository = matchRepository;
        }

        /// <summary>
        /// Asynchronously confirms a booking for a candidate by updating the slot's status to occupied and creating a new interview session.
        /// </summary>
        /// <param name="slotId">The unique identifier of the slot to be booked.</param>
        /// <param name="candidateId">The unique identifier of the candidate making the booking.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the slot is not found.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the slot is no longer available.</exception>
        public async Task ConfirmBookingAsync(int slotId, int candidateId, int jobId)
        {
            Slot? slot = await this._slotRepository.GetByIdAsync(slotId);
            if (slot == null)
            {
                throw new KeyNotFoundException("Slot not found.");
            }

            if (slot.Status != SlotStatus.Free)
            {
                throw new InvalidOperationException("This slot is no longer available.");
            }

            slot.Status = SlotStatus.Occupied;
            slot.CandidateId = candidateId;
            slot.InterviewType = string.Empty;

            await this._slotRepository.UpdateAsync(slot);

            InterviewSession newInterviewSession = new InterviewSession
            {
                PositionId = jobId,
                ExternalUserId = candidateId,
                InterviewerId = slot.RecruiterId,
                DateStart = slot.StartTime.ToUniversalTime(),
                Video = string.Empty,
                Status = InterviewStatus.Scheduled.ToString(),
                Score = MINIMUMINTERVIEWSCORE,
            };

            this._interviewSessionRepository.Add(newInterviewSession);

            var match = await this._matchRepository.GetByUserIdAndJobIdAsync(candidateId, jobId);
            if (match != null)
            {
                match.Status = MatchStatus.Advanced;
                await this._matchRepository.UpdateAsync(match);
            }
        }
    }
}
