// <copyright file="UpdateSlotDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.DTOs
{
    /// <summary>
    /// Data transfer object for updating a recruiter slot.
    /// </summary>
    public class UpdateSlotDto
    {
        /// <summary>Gets or sets the initial slot to be updated.</summary>
        public SlotDto InitialSlot { get; set; } = new SlotDto();

        /// <summary>Gets or sets the new start time for the slot.</summary>
        public DateTime StartTime { get; set; }

        /// <summary>Gets or sets the new duration in minutes.</summary>
        public int Duration { get; set; }
    }
}