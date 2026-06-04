using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Domain.Core;

namespace UBB_SE_2026_Jobs.Library.Mappers
{
    /// <summary>
    /// Provides extension methods for mapping between TestQuestion and QuestionDto objects.
    /// </summary>
    public static class QuestionMapper
    {
        /// <summary>
        /// Converts a TestQuestion entity to its corresponding QuestionDto representation.
        /// </summary>
        /// <param name="entity">The TestQuestion entity to convert. Cannot be null.</param>
        /// <returns>A QuestionDto object containing the data from the specified TestQuestion entity.</returns>
        public static QuestionDto ToDto(this TestQuestion entity)
        {
            return new QuestionDto
            {
                Id = entity.Id,
                QuestionText = entity.QuestionText,
                QuestionType = entity.QuestionTypeString,
                QuestionScore = entity.QuestionScore,
                QuestionAnswer = entity.QuestionAnswer,
                OptionsJson = entity.OptionsJson,
                TestId = entity.TestId
            };
        }

        /// <summary>
        /// Converts a QuestionDto instance to its corresponding TestQuestion entity.
        /// </summary>
        /// <param name="dto">The QuestionDto object to convert. Cannot be null.</param>
        /// <returns>A new TestQuestion entity populated with values from the specified QuestionDto.</returns>
        public static TestQuestion ToEntity(this QuestionDto dto)
        {
            return new TestQuestion
            {
                Id = dto.Id,
                QuestionText = dto.QuestionText,
                QuestionTypeString = dto.QuestionType,
                QuestionScore = dto.QuestionScore,
                QuestionAnswer = dto.QuestionAnswer,
                OptionsJson = dto.OptionsJson,
                TestId = dto.TestId
            };
        }
    }
}