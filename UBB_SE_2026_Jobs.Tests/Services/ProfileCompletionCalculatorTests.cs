using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class ProfileCompletionCalculatorTests
{
    private readonly ITestsJobsRepository jobsRepository = Substitute.For<ITestsJobsRepository>();
    private readonly IApplicantRepository applicantRepository = Substitute.For<IApplicantRepository>();
    private readonly ProfileCompletionCalculator profileCompletionCalculator;

    private const int TotalRequiredTasksCount = 5;
    private const int PercentageMultiplier = 100;
    private const int MinimumRequiredPostedJobs = 5;
    private const int MinimumRequiredCollaborators = 2;

    public ProfileCompletionCalculatorTests()
    {
        profileCompletionCalculator = new ProfileCompletionCalculator(jobsRepository, applicantRepository);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Company EmptyCompany() => new()
    {
        ProfilePicturePath = null,
        AboutUs = null,
        PostedJobsCount = 0,
        CollaboratorsCount = 0,
        Game = null,
    };

    private static Company FullyCompleteCompany() => new()
    {
        ProfilePicturePath = "https://example.com/pic.jpg",
        AboutUs = "We build great things.",
        PostedJobsCount = MinimumRequiredPostedJobs,
        CollaboratorsCount = MinimumRequiredCollaborators,
        Game = new Game { IsPublished = true },
    };

    private static int ExpectedPercentage(int completedTasks) =>
        completedTasks * PercentageMultiplier / TotalRequiredTasksCount;

    private static Applicant AppliedThisWeek() => new() { AppliedAt = DateTime.Now.AddDays(-3) };

    private static Applicant AppliedLastWeek() => new() { AppliedAt = DateTime.Now.AddDays(-10) };

    // Tests

    [Fact]
    public void Calculate_NoTasksCompleted_ReturnsZeroPercent()
    {
        var (percentage, _) = profileCompletionCalculator.Calculate(EmptyCompany());

        Assert.Equal(ExpectedPercentage(0), percentage);
    }

    [Fact]
    public void Calculate_AllTasksCompleted_Returns100Percent()
    {
        var (percentage, _) = profileCompletionCalculator.Calculate(FullyCompleteCompany());

        Assert.Equal(100, percentage);
    }


    [Theory]
    [InlineData(nameof(Company.ProfilePicturePath))]
    [InlineData(nameof(Company.AboutUs))]
    [InlineData(nameof(Company.PostedJobsCount))]
    [InlineData(nameof(Company.CollaboratorsCount))]
    [InlineData(nameof(Company.Game))]
    public void Calculate_ExactlyOneTaskCompleted_Returns20Percent(string completedTaskProperty)
    {
        var company = EmptyCompany();

        switch (completedTaskProperty)
        {
            case nameof(Company.ProfilePicturePath):
                company.ProfilePicturePath = "https://example.com/pic.jpg";
                break;
            case nameof(Company.AboutUs):
                company.AboutUs = "We build great things.";
                break;
            case nameof(Company.PostedJobsCount):
                company.PostedJobsCount = MinimumRequiredPostedJobs;
                break;
            case nameof(Company.CollaboratorsCount):
                company.CollaboratorsCount = MinimumRequiredCollaborators;
                break;
            case nameof(Company.Game):
                company.Game = new Game { IsPublished = true };
                break;
        }

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(1), percentage);
    }


    [Fact]
    public void Calculate_PostedJobsOneBelowMinimum_DoesNotCountAsCompleted()
    {
        var company = EmptyCompany();
        company.PostedJobsCount = MinimumRequiredPostedJobs - 1;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(0), percentage);
    }

    [Fact]
    public void Calculate_CollaboratorsOneBelowMinimum_DoesNotCountAsCompleted()
    {
        var company = EmptyCompany();
        company.CollaboratorsCount = MinimumRequiredCollaborators - 1;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(0), percentage);
    }

    [Fact]
    public void Calculate_GameExistsButNotPublished_DoesNotCountAsCompleted()
    {
        var company = EmptyCompany();
        company.Game = new Game { IsPublished = false };

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(0), percentage);
    }


    [Fact]
    public void Calculate_NoTasksCompleted_AllFiveTasksAreInRemainingList()
    {
        var (_, remainingTasks) = profileCompletionCalculator.Calculate(EmptyCompany());

        Assert.Equal(TotalRequiredTasksCount, remainingTasks.Count);
        Assert.Contains("Upload company picture", remainingTasks);
        Assert.Contains("Add company description", remainingTasks);
        Assert.Contains("Post at least 5 jobs", remainingTasks);
        Assert.Contains("Add 2 collaborators", remainingTasks);
        Assert.Contains("Complete mini-game", remainingTasks);
    }

    [Fact]
    public void Calculate_AllTasksCompleted_RemainingListIsEmpty()
    {
        var (_, remainingTasks) = profileCompletionCalculator.Calculate(FullyCompleteCompany());

        Assert.Empty(remainingTasks);
    }

    [Fact]
    public void Calculate_CompletedTasksAreNotInRemainingList()
    {
        var company = FullyCompleteCompany();
        company.ProfilePicturePath = null;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Single(remainingTasks);
        Assert.Contains("Upload company picture", remainingTasks);
    }

    [Fact]
    public void Calculate_RemainingListCountMatchesIncompleteTasks()
    {
        var company = EmptyCompany();
        company.ProfilePicturePath = "https://example.com/pic.jpg";
        company.AboutUs = "We build great things.";
        company.PostedJobsCount = MinimumRequiredPostedJobs;

        var (percentage, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(3), percentage);
        Assert.Equal(2, remainingTasks.Count);
    }


    [Fact]
    public void GetSkillsTop3_NoJobsForCompany_ReturnsBothListsEmpty()
    {
        jobsRepository.GetAllJobs().Returns(new List<Job>());

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Empty(skillNames);
        Assert.Empty(percentages);
    }

    [Fact]
    public void GetSkillsTop3_JobsBelongToOtherCompany_AreExcluded()
    {
        var jobFromOtherCompany = new Job
        {
            Company = new Company { CompanyId = 99 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 50 }
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { jobFromOtherCompany });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Empty(skillNames);
    }

    [Fact]
    public void GetSkillsTop3_JobWithNullSkillsList_IsIgnoredGracefully()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = null,
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Empty(skillNames);
    }

    [Fact]
    public void GetSkillsTop3_SkillWithEmptyName_IsExcludedFromResults()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "" },  RequiredPercentage = 50 },
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 50 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Single(skillNames);
        Assert.Equal("C#", skillNames[0]);
    }


    [Fact]
    public void GetSkillsTop3_FourSkillsAvailable_OnlyTopThreeReturned()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },     RequiredPercentage = 40 },
                new() { Skill = new Skill { SkillName = "SQL" },    RequiredPercentage = 30 },
                new() { Skill = new Skill { SkillName = "Azure" },  RequiredPercentage = 20 },
                new() { Skill = new Skill { SkillName = "Docker" }, RequiredPercentage = 10 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Equal(3, skillNames.Count);
        Assert.Equal(3, percentages.Count);
    }

    [Fact]
    public void GetSkillsTop3_SkillsReturnedInDescendingOrderOfWeight()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "Docker" }, RequiredPercentage = 10 },
                new() { Skill = new Skill { SkillName = "C#" },     RequiredPercentage = 80 },
                new() { Skill = new Skill { SkillName = "SQL" },    RequiredPercentage = 20 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Equal("C#", skillNames[0]);
        Assert.Equal("SQL", skillNames[1]);
        Assert.Equal("Docker", skillNames[2]);
    }

    [Fact]
    public void GetSkillsTop3_FewerThanThreeSkillsAvailable_ReturnsOnlyWhatExists()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },  RequiredPercentage = 80 },
                new() { Skill = new Skill { SkillName = "SQL" }, RequiredPercentage = 20 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Equal(2, skillNames.Count);
        Assert.Equal(2, percentages.Count);
    }


    [Fact]
    public void GetSkillsTop3_SkillPercentagesAreNormalisedRelativeToAllSkills()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },  RequiredPercentage = 80 },
                new() { Skill = new Skill { SkillName = "SQL" }, RequiredPercentage = 20 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Equal(80, percentages[skillNames.IndexOf("C#")]);
        Assert.Equal(20, percentages[skillNames.IndexOf("SQL")]);
    }

    [Fact]
    public void GetSkillsTop3_NormalisationExcludesSkillsBeyondTopThree()
    {
        var job = new Job
        {
            Company = new Company { CompanyId = 1 },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },     RequiredPercentage = 40 },
                new() { Skill = new Skill { SkillName = "SQL" },    RequiredPercentage = 30 },
                new() { Skill = new Skill { SkillName = "Azure" },  RequiredPercentage = 20 },
                new() { Skill = new Skill { SkillName = "Docker" }, RequiredPercentage = 10 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (_, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        // Total of displayed percentages is less than 100 because Docker is hidden
        Assert.True(percentages.Sum() < 100);
    }

    [Fact]
    public void GetSkillsTop3_SameSkillInMultipleJobs_RequiredPercentageIsAccumulated()
    {
        var company = new Company { CompanyId = 1 };
        var firstJob = new Job
        {
            Company = company,
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 40 },
            }
        };
        var secondJob = new Job
        {
            Company = company,
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 60 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { firstJob, secondJob });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Single(skillNames);
        Assert.Equal(100, percentages[0]);
    }


    [Fact]
    public void ApplicantsMessage_NoApplicantsAtAll_ReturnsPromptToPostJobs()
    {
        applicantRepository.GetApplicantsByCompany(1).Returns(new List<Applicant>());

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("No applicants yet. Start posting jobs!", message);
    }

    [Fact]
    public void ApplicantsMessage_ApplicantsThisWeekButNonePreviously_ReturnsGreatStartWithCount()
    {
        var applicants = new List<Applicant> { AppliedThisWeek(), AppliedThisWeek() };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("Great start! You have 2 new applicants.", message);
    }

    [Fact]
    public void ApplicantsMessage_DoubleTheApplicantsComparedToLastWeek_Shows100PercentIncrease()
    {
        var applicants = new List<Applicant>
        {
            AppliedThisWeek(),
            AppliedThisWeek(),
            AppliedLastWeek(),
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("Congrats! You have 100% more applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_HalfTheApplicantsComparedToLastWeek_Shows50PercentDecrease()
    {
        var applicants = new List<Applicant>
        {
            AppliedThisWeek(),
            AppliedLastWeek(),
            AppliedLastWeek(),
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("You have 50% fewer applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_SameCountAsLastWeek_ShowsZeroPercentIncrease()
    {
        var applicants = new List<Applicant> { AppliedThisWeek(), AppliedLastWeek() };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("Congrats! You have 0% more applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_AllApplicantsFromLastWeekNoneThisWeek_Shows100PercentDecrease()
    {
        var applicants = new List<Applicant> { AppliedLastWeek(), AppliedLastWeek() };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("You have 100% fewer applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_ApplicantsRightAtThe7DayBoundary_CountedAsThisWeek()
    {
        var boundaryApplicant = new Applicant { AppliedAt = DateTime.Now.AddDays(-7).AddMinutes(1) };
        applicantRepository.GetApplicantsByCompany(1).Returns(new List<Applicant> { boundaryApplicant });

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.StartsWith("Great start!", message);
    }
}