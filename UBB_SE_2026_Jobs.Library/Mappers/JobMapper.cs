namespace UBB_SE_2026_Jobs.Library.Mappers
{
    using System.Collections.Generic;
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Domain;

    /// <summary>
    /// Provides extension methods for mapping between Job and JobDto objects.
    /// </summary>
    public static class JobMapper
    {
        /// <summary>
        /// Converts a Job entity to its corresponding JobDto representation.
        /// </summary>
        /// <param name="entity">The Job entity to convert. Cannot be null.</param>
        /// <returns>A JobDto object containing the data from the specified Job entity.</returns>
        public static JobDto ToDto(this Job entity)
        {
            return new JobDto
            {
                JobId = entity.JobId,
                CompanyId = entity.CompanyId,
                Photo = entity.Photo,
                JobTitle = entity.JobTitle,
                IndustryField = entity.IndustryField,
                JobType = entity.JobType,
                ExperienceLevel = entity.ExperienceLevel,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                JobDescription = entity.JobDescription,
                JobLocation = entity.JobLocation,
                AvailablePositions = entity.AvailablePositions,
                PostedAt = entity.PostedAt,
                Salary = entity.Salary,
                AmountPayed = entity.AmountPayed,
                Deadline = entity.Deadline,
                JobSkills = entity.JobSkills.Select(skill => skill.ToDto()).ToList(),
            };
        }

        /// <summary>
        /// Converts a JobDto instance to its corresponding Job entity.
        /// </summary>
        /// <param name="dto">The JobDto object to convert. Cannot be null.</param>
        /// <returns>A new Job entity populated with values from the specified JobDto.</returns>
        public static Job ToEntity(this JobDto dto)
        {
            return new Job
            {
                JobId = dto.JobId,
                CompanyId = dto.CompanyId,
                Photo = dto.Photo,
                JobTitle = dto.JobTitle,
                IndustryField = dto.IndustryField,
                JobType = dto.JobType,
                ExperienceLevel = dto.ExperienceLevel,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                JobDescription = dto.JobDescription,
                JobLocation = dto.JobLocation,
                AvailablePositions = dto.AvailablePositions,
                PostedAt = dto.PostedAt,
                Salary = dto.Salary,
                AmountPayed = dto.AmountPayed,
                Deadline = dto.Deadline,
                JobSkills = new List<JobSkill>(),
            };
        }
    }
}