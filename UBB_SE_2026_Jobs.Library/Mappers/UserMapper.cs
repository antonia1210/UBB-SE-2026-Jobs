namespace UBB_SE_2026_Jobs.Library.Mappers
{
    using UBB_SE_2026_Jobs.Library.DTOs;
    using UBB_SE_2026_Jobs.Library.Domain;

    /// <summary>
    /// Provides extension methods for mapping between User and UserDto objects.
    /// </summary>
    public static class UserMapper
    {
        /// <summary>
        /// Converts a User entity to its corresponding UserDto representation.
        /// </summary>
        /// <param name="entity">The User entity to convert. Cannot be null.</param>
        /// <returns>A UserDto object containing the data from the specified User entity.</returns>
        public static UserDto ToDto(this User entity)
        {
            return new UserDto
            {
                Id = entity.UserId,
                Name = entity.Name,
                Email = entity.Email,
                CvXml = entity.ParsedCv,
                PasswordHash = entity.PasswordHash,
                // Role might need handling if it's missing from User entity
            };
        }

        /// <summary>
        /// Converts a UserDto instance to its corresponding User entity.
        /// </summary>
        /// <param name="dto">The UserDto object to convert. Cannot be null.</param>
        /// <returns>A new User entity populated with values from the specified UserDto.</returns>
        public static User ToEntity(this UserDto dto)
        {
            var parts = dto.Name.Split(' ', 2);
            var firstName = parts.Length > 0 ? parts[0] : dto.Name;
            var lastName = parts.Length > 1 ? parts[1] : string.Empty;
            
            return new User
            {
                UserId = dto.Id,
                FirstName = firstName,
                LastName = lastName,
                Email = dto.Email,
                PasswordHash = dto.PasswordHash,
                ParsedCv = dto.CvXml ?? string.Empty
            };
        }
    }
}