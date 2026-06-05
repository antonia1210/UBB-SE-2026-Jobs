namespace UBB_SE_2026_Jobs.Library.Services
{
    /// <summary>
    /// Outcome of attempting to delete a job posting.
    /// </summary>
    public enum JobDeleteResult
    {
        /// <summary>No job exists with the requested id.</summary>
        NotFound,

        /// <summary>
        /// The job has applicants (Match records) and was not deleted because the caller did not
        /// request a forced (cascade) delete.
        /// </summary>
        HasApplicants,

        /// <summary>The job (and, when forced, its applicants) was deleted.</summary>
        Deleted,
    }
}
