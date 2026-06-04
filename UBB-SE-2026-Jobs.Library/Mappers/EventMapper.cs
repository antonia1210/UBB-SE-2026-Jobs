namespace UBB_SE_2026_Jobs.Library.Mappers
{
    using UBB_SE_2026_Jobs.Library.DTOs.Portal;
    using UBB_SE_2026_Jobs.Library.Domain.Portal;

    /// <summary>
    /// Provides extension methods for mapping between Event and EventDto objects.
    /// </summary>
    public static class EventMapper
    {
        /// <summary>
        /// Converts an Event entity to its corresponding EventDto representation.
        /// </summary>
        /// <param name="entity">The Event entity to convert. Cannot be null.</param>
        /// <returns>An EventDto object containing the data from the specified Event entity.</returns>
        public static EventDto ToDto(this Event entity)
        {
            return new EventDto
            {
                Id = entity.Id,
                Photo = entity.Photo,
                Title = entity.Title,
                Description = entity.Description,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Location = entity.Location,
                HostCompanyId = entity.HostCompanyId,
                PostedAt = entity.PostedAt,
            };
        }

        /// <summary>
        /// Converts an EventDto instance to its corresponding Event entity.
        /// </summary>
        /// <param name="dto">The EventDto object to convert. Cannot be null.</param>
        /// <returns>A new Event entity populated with values from the specified EventDto.</returns>
        public static Event ToEntity(this EventDto dto)
        {
            return new Event
            {
                Photo = dto.Photo,
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Location = dto.Location,
                HostCompanyId = dto.HostCompanyId,
                PostedAt = dto.PostedAt,
            };
        }
    }
}