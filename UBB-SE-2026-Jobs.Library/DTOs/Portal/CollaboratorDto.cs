namespace UBB_SE_2026_Jobs.Library.DTOs.Portal
{
    /// <summary>
    /// Represents the association between an event and a collaborating company.
    /// </summary>
    public class CollaboratorDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the event associated with the collaboration.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the company associated with the collaboration.
        /// </summary>
        public int CompanyId { get; set; }
    }
}