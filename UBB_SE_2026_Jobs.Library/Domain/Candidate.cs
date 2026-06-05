// <copyright file="Candidate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace UBB_SE_2026_Jobs.Library.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a candidate in the recruitment system, including application status, assigned recruiter, matched
    /// company, and available booking slots.
    /// </summary>
    [NotMapped]
    public class Candidate
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the recruiter assigned to this entity.
        /// </summary>
        public int AssignedRecruiterId { get; set; }

        /// <summary>
        /// Gets or sets the current status of the application.
        /// </summary>
        public string ApplicationStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the company that was matched during processing.
        /// </summary>
        public string MatchedCompany { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of available slots.
        /// </summary>
        [NotMapped]
        public List<Slot> AvailableSlots { get; set; } = new List<Slot>();


    }
}
