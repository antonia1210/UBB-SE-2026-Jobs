namespace UBB_SE_2026_Jobs.Library.Mappers
{
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

    /// <summary>
    /// Provides extension methods for mapping between JobPaymentInfo and JobPaymentInfoDto objects.
    /// </summary>
    public static class JobPaymentInfoMapper
    {
        /// <summary>
        /// Converts a JobPaymentInfo entity to its corresponding JobPaymentInfoDto representation.
        /// </summary>
        /// <param name="entity">The JobPaymentInfo entity to convert. Cannot be null.</param>
        /// <returns>A JobPaymentInfoDto object containing the data from the specified JobPaymentInfo entity.</returns>
        public static JobPaymentInfoDto ToDto(this JobPaymentInfo entity)
        {
            return new JobPaymentInfoDto
            {
                CompanyName = entity.CompanyName,
                JobTitle = entity.JobTitle,
                AmountPayed = entity.AmountPayed,
            };
        }

        /// <summary>
        /// Converts a JobPaymentInfoDto instance to its corresponding JobPaymentInfo entity.
        /// </summary>
        /// <param name="dto">The JobPaymentInfoDto object to convert. Cannot be null.</param>
        /// <returns>A new JobPaymentInfo entity populated with values from the specified JobPaymentInfoDto.</returns>
        public static JobPaymentInfo ToEntity(this JobPaymentInfoDto dto)
        {
            return new JobPaymentInfo
            {
                CompanyName = dto.CompanyName,
                JobTitle = dto.JobTitle,
                AmountPayed = dto.AmountPayed,
            };
        }
    }
}