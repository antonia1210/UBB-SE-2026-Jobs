using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services
{
    public class CompanyStatsServiceTests
    {
        private readonly ITestsJobsRepository jobsRepository;
        private readonly IApplicantRepository applicantRepository;
        private readonly CompanyStatsService service;

        public CompanyStatsServiceTests()
        {
            jobsRepository = Substitute.For<ITestsJobsRepository>();
            applicantRepository = Substitute.For<IApplicantRepository>();
            service = new CompanyStatsService(jobsRepository, applicantRepository);
        }

        [Fact]
        public async Task GetSkillsTop3Async_NoJobsFound_ReturnsEmptyLists()
        {
            jobsRepository.GetAllJobs().Returns(new List<Job>());

            var (names, percents) = await service.GetSkillsTop3Async(1);

            Assert.Empty(names);
            Assert.Empty(percents);
        }

        [Fact]
        public async Task GetSkillsTop3Async_JobsHaveNoSkills_ReturnsEmptyLists()
        {
            var jobs = new List<Job> { new Job { CompanyId = 1, JobSkills = null } };
            jobsRepository.GetAllJobs().Returns(jobs);

            var (names, percents) = await service.GetSkillsTop3Async(1);

            Assert.Empty(names);
            Assert.Empty(percents);
        }

        [Fact]
        public async Task GetSkillsTop3Async_ValidSkills_ReturnsTop3SortedWithPercentages()
        {
            var jobs = new List<Job>
            {
                new Job
                {
                    CompanyId = 1,
                    JobSkills = new List<JobSkill>
                    {
                        new JobSkill { RequiredPercentage = 50, Skill = new Skill { SkillName = "C#" } },
                        new JobSkill { RequiredPercentage = 30, Skill = new Skill { SkillName = "SQL" } }
                    }
                },
                new Job
                {
                    CompanyId = 1,
                    JobSkills = new List<JobSkill>
                    {
                        new JobSkill { RequiredPercentage = 10, Skill = new Skill { SkillName = "C#" } },
                        new JobSkill { RequiredPercentage = 10, Skill = new Skill { SkillName = "Git" } },
                        new JobSkill { RequiredPercentage = 100, Skill = new Skill { SkillName = "Docker" } }
                    }
                },
                new Job
                {
                    CompanyId = 2,
                    JobSkills = new List<JobSkill>
                    {
                        new JobSkill { RequiredPercentage = 500, Skill = new Skill { SkillName = "Python" } }
                    }
                }
            };
            jobsRepository.GetAllJobs().Returns(jobs);

            var (names, percents) = await service.GetSkillsTop3Async(1);

            Assert.Equal(3, names.Count);
            Assert.Equal("Docker", names[0]);
            Assert.Equal("C#", names[1]);
            Assert.Equal("SQL", names[2]);

            Assert.Equal(50, percents[0]); // 100 / 200 *100
            Assert.Equal(30, percents[1]); // 60 / 200 *100
            Assert.Equal(15, percents[2]); // 30 / 200 *100
        }

        [Fact]
        public async Task ApplicantsMessageAsync_NoApplicants_ReturnsNoApplicantsText()
        {
            applicantRepository.GetApplicantsByCompany(1).Returns(new List<Applicant>());

            var result = await service.ApplicantsMessageAsync(1);

            Assert.Equal("No applicants yet. Start posting jobs!", result);
        }

        [Fact]
        public async Task ApplicantsMessageAsync_OnlyCurrentWeekApplicants_ReturnsGreatStartText()
        {
            var applicants = new List<Applicant>
            {
                new Applicant { AppliedAt = DateTime.Now.AddDays(-2) },
                new Applicant { AppliedAt = DateTime.Now.AddDays(-1) }
            };
            applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

            var result = await service.ApplicantsMessageAsync(1);

            Assert.Equal("Great start! You have 2 new applicants.", result);
        }

        [Fact]
        public async Task ApplicantsMessageAsync_FewerApplicantsThanLastWeek_ReturnsFewerApplicantsText()
        {
            var applicants = new List<Applicant>
            {
                new Applicant { AppliedAt = DateTime.Now.AddDays(-10) },
                new Applicant { AppliedAt = DateTime.Now.AddDays(-9) },
                new Applicant { AppliedAt = DateTime.Now.AddDays(-1) }
            };
            applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

            var result = await service.ApplicantsMessageAsync(1);

            Assert.Equal("You have 50% fewer applicants than last week.", result);
        }

        [Fact]
        public async Task ApplicantsMessageAsync_MoreApplicantsThanLastWeek_ReturnsMoreApplicantsText()
        {
            var applicants = new List<Applicant>
            {
                new Applicant { AppliedAt = DateTime.Now.AddDays(-10) },
                new Applicant { AppliedAt = DateTime.Now.AddDays(-1) },
                new Applicant { AppliedAt = DateTime.Now.AddDays(-2) },
                new Applicant { AppliedAt = DateTime.Now }
            };
            applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

            var result = await service.ApplicantsMessageAsync(1);

            Assert.Equal("Congrats! You have 200% more applicants than last week.", result);
        }
    }
}