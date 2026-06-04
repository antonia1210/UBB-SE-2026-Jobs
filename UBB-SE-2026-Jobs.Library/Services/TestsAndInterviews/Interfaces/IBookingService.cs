// <copyright file="IBookingService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.TestsAndInterviews.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models;
    using UBB_SE_2026_Jobs.Library.TestsAndInterviews.Models.Core;

    /// <summary>
    /// Defines the contract for booking-related operations.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Asynchronously confirms a booking for a candidate by updating the slot's status to occupied and creating a new interview session.
        /// </summary>
        /// <param name="slotId">The unique identifier of the slot to be booked.</param>
        /// <param name="candidateId">The unique identifier of the candidate making the booking.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ConfirmBookingAsync(int slotId, int candidateId);
    }
}
