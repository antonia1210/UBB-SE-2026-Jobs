using System;
using Xunit;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.RecommendationAlgorithm;
using UBB_SE_2026_Jobs.Tests.Helpers;

namespace UBB_SE_2026_Jobs.Tests.Algorithm
{
    public class RecommendationAlgorithmAdditionalTests
    {
        private readonly RecommendationAlgorithm algorithm = new();


        [Fact]
        public void SkillScore_UserMeetsRequiredLevel_NoPenalty()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = new[] { BuildUserSkill(skillId: 1, score: 70, name: "Python") };
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 70, name: "Python") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(100d, breakdown.SkillScore, 1);
        }

        [Fact]
        public void SkillScore_UserExceedsRequiredLevel_PenaltyIsHalved()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = new[] { BuildUserSkill(skillId: 1, score: 90, name: "Java") };
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 70, name: "Java") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(90d, breakdown.SkillScore, 1);
        }

        [Fact]
        public void SkillScore_UserBelowRequiredLevel_FullPenaltyApplied()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = new[] { BuildUserSkill(skillId: 1, score: 40, name: "Go") };
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 70, name: "Go") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(70d, breakdown.SkillScore, 1);
        }

        [Fact]
        public void SkillScore_UserLacksSkillEntirely_TreatedAsZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = Array.Empty<UserSkill>();
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 60, name: "Rust") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(40d, breakdown.SkillScore, 1);
        }

        [Fact]
        public void SkillScore_MatchedByNameWhenIdsDiffer_SkillIsFound()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = new[] { BuildUserSkill(skillId: 99, score: 80, name: "TypeScript") };
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 80, name: "TypeScript") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(100d, breakdown.SkillScore, 1);
        }

        [Fact]
        public void SkillScore_PenaltyCannotDropScoreBelowZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var userSkills = Array.Empty<UserSkill>();
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 200, name: "Haskell") };

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, userSkills, jobSkills);

            Assert.Equal(0d, breakdown.SkillScore, 1);
        }


        [Fact]
        public void KeywordScore_NoOverlapBetweenCvAndDescription_ReturnsZero()
        {
            var user = BuildUser(parsedCv: "python django", location: "X", employment: "X");
            var job = BuildJob(description: "java spring", location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, breakdown.KeywordScore, 1);
        }

        [Fact]
        public void KeywordScore_CompleteOverlapBetweenCvAndDescription_Returns100()
        {
            var user = BuildUser(parsedCv: "kotlin coroutines", location: "X", employment: "X");
            var job = BuildJob(description: "kotlin coroutines", location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(100d, breakdown.KeywordScore, 1);
        }

        [Fact]
        public void KeywordScore_BothCvAndDescriptionEmpty_ReturnsZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, breakdown.KeywordScore, 1);
        }

        [Fact]
        public void KeywordScore_IsCaseInsensitive()
        {
            var user = BuildUser(parsedCv: "SQL", location: "X", employment: "X");
            var job = BuildJob(description: "sql", location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(100d, breakdown.KeywordScore, 1);
        }

        [Fact]
        public void KeywordScore_PunctuationIsIgnored()
        {
            var user = BuildUser(parsedCv: "C#", location: "X", employment: "X");
            var job = BuildJob(description: "C#,", location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(100d, breakdown.KeywordScore, 1);
        }


        [Fact]
        public void PreferenceScore_OnlyLocationMatches_Returns50()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "Cluj-Napoca", employment: "Full-time");
            var job = BuildJob(description: string.Empty, location: "Cluj-Napoca", employment: "Part-time", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(50d, breakdown.PreferenceScore, 1);
        }

        [Fact]
        public void PreferenceScore_OnlyEmploymentTypeMatches_Returns50()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "Bucharest", employment: "Remote");
            var job = BuildJob(description: string.Empty, location: "Cluj-Napoca", employment: "Remote", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(50d, breakdown.PreferenceScore, 1);
        }

        [Fact]
        public void PreferenceScore_NeitherMatches_ReturnsZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "Bucharest", employment: "Full-time");
            var job = BuildJob(description: string.Empty, location: "Cluj-Napoca", employment: "Part-time", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, breakdown.PreferenceScore, 1);
        }

        [Fact]
        public void PreferenceScore_PreferenceMatchIsCaseInsensitive()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "cluj-napoca", employment: "full-time");
            var job = BuildJob(description: string.Empty, location: "Cluj-Napoca", employment: "Full-time", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(100d, breakdown.PreferenceScore, 1);
        }


        [Fact]
        public void PromotionScore_ZeroPromotionLevel_ReturnsZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: 0);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, breakdown.PromotionScore, 1);
        }

        [Fact]
        public void PromotionScore_NegativePromotionLevel_ClampedToZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "X", employment: "X");
            var job = BuildJob(description: string.Empty, location: "X", employment: "X", promotion: -50);

            var breakdown = algorithm.CalculateScoreBreakdown(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, breakdown.PromotionScore, 1);
        }

        [Fact]
        public void OverallScore_AllComponentsZero_ReturnsZero()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "Bucharest", employment: "Full-time");
            var job = BuildJob(description: string.Empty, location: "Cluj-Napoca", employment: "Part-time", promotion: 0);

            var score = algorithm.CalculateCompatibilityScore(user, job, Array.Empty<UserSkill>(), Array.Empty<JobSkill>());

            Assert.Equal(0d, score, 1);
        }

        [Fact]
        public void OverallScore_IsNeverAbove100()
        {
            var user = BuildUser(parsedCv: "a b c", location: "X", employment: "Full-time");
            var job = BuildJob(description: "a b c", location: "X", employment: "Full-time", promotion: 200);

            var userSkills = new[] { BuildUserSkill(skillId: 1, score: 100, name: "Skill") };
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 50, name: "Skill") };

            var score = algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills);

            Assert.True(score <= 100d);
        }

        [Fact]
        public void OverallScore_IsNeverBelow0()
        {
            var user = BuildUser(parsedCv: string.Empty, location: "nowhere", employment: "nowhere");
            var job = BuildJob(description: string.Empty, location: "somewhere", employment: "something", promotion: -999);

            var userSkills = Array.Empty<UserSkill>();
            var jobSkills = new[] { BuildJobSkill(skillId: 1, requiredLevel: 100, name: "X") };

            var score = algorithm.CalculateCompatibilityScore(user, job, userSkills, jobSkills);

            Assert.True(score >= 0d);
        }


        private static User BuildUser(string parsedCv, string location, string employment)
        {
            var user = new UserBuilder().WithParsedCv(parsedCv).Build();
            user.LocationPreference = location;
            user.PreferredEmploymentType = employment;
            return user;
        }

        private static Job BuildJob(string description, string location, string employment, int promotion)
        {
            var job = new JobBuilder()
                .WithTitle("T")
                .WithLocation(location)
                .WithEmploymentType(employment)
                .WithPromotionLevel(promotion)
                .Build();

            job.JobDescription = description;
            job.JobLocation = location;
            job.JobType = employment;
            job.PromotionLevel = promotion;
            return job;
        }

        private static UserSkill BuildUserSkill(int skillId, int score, string name) =>
            new UserSkill
            {
                User = new User { UserId = 1 },
                Score = score,
                Skill = new Skill { SkillId = skillId, Name = name }
            };

        private static JobSkill BuildJobSkill(int skillId, int requiredLevel, string name) =>
            new JobSkill
            {
                Job = new Job { JobId = 1 },
                RequiredLevel = requiredLevel,
                Skill = new Skill { SkillId = skillId, Name = name }
            };
    }
}