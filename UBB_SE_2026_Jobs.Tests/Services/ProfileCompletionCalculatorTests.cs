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

    // Helpers

    private static Company EmptyCompany() => new()
    {
        ProfilePicturePath = null,
        AboutUs = null,
        PostedJobsCount = 0,
        CollaboratorsCount = 0,
        Game = null,
    };

    private static Game CreatePublishedGame()
    {
        Game game = new Game();
        game.Publish();
        return game;
    }

    private static Company FullyCompleteCompany() => new()
    {
        ProfilePicturePath = "https://example.com/pic.jpg",
        AboutUs = "We build great things.",
        PostedJobsCount = MinimumRequiredPostedJobs,
        CollaboratorsCount = MinimumRequiredCollaborators,
        Game = CreatePublishedGame(),
    };

    private static int ExpectedPercentage(int completedTasks) =>
        completedTasks * PercentageMultiplier / TotalRequiredTasksCount;

    // -------------------------------------------------------------------------
    // Calculate — percentage
    // -------------------------------------------------------------------------

    [Fact]
    public void Calculate_NoTasksCompleted_ReturnsZeroPercent()
    {
        var company = EmptyCompany();

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 0), percentage);
    }

    [Fact]
    public void Calculate_AllTasksCompleted_ReturnsOneHundredPercent()
    {
        var company = FullyCompleteCompany();

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 5), percentage);
    }

    [Fact]
    public void Calculate_ProfilePicturePresent_CountsAsOneCompletedTask()
    {
        var company = EmptyCompany();
        company.ProfilePicturePath = "https://example.com/pic.jpg";

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 1), percentage);
    }

    [Fact]
    public void Calculate_AboutUsPresent_CountsAsOneCompletedTask()
    {
        var company = EmptyCompany();
        company.AboutUs = "We build great things.";

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 1), percentage);
    }

    [Fact]
    public void Calculate_PostedJobsAtMinimum_CountsAsOneCompletedTask()
    {
        var company = EmptyCompany();
        company.PostedJobsCount = MinimumRequiredPostedJobs;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 1), percentage);
    }

    [Fact]
    public void Calculate_PostedJobsBelowMinimum_DoesNotCountTask()
    {
        var company = EmptyCompany();
        company.PostedJobsCount = MinimumRequiredPostedJobs - 1;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 0), percentage);
    }

    [Fact]
    public void Calculate_CollaboratorsAtMinimum_CountsAsOneCompletedTask()
    {
        var company = EmptyCompany();
        company.CollaboratorsCount = MinimumRequiredCollaborators;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 1), percentage);
    }

    [Fact]
    public void Calculate_CollaboratorsBelowMinimum_DoesNotCountTask()
    {
        var company = EmptyCompany();
        company.CollaboratorsCount = MinimumRequiredCollaborators - 1;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 0), percentage);
    }

    [Fact]
    public void Calculate_PublishedGamePresent_CountsAsOneCompletedTask()
    {
        var company = EmptyCompany();
        company.Game = CreatePublishedGame();

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 1), percentage);
    }

    [Fact]
    public void Calculate_UnpublishedGame_DoesNotCountTask()
    {
        var company = EmptyCompany();
        company.Game = new Game();

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 0), percentage);
    }

    [Fact]
    public void Calculate_NullGame_DoesNotCountTask()
    {
        var company = EmptyCompany();
        company.Game = null;

        var (percentage, _) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(ExpectedPercentage(completedTasks: 0), percentage);
    }

    // Calculate — remaining tasks list

    [Fact]
    public void Calculate_NoTasksCompleted_ReturnsAllFiveRemainingTasks()
    {
        var company = EmptyCompany();

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Equal(TotalRequiredTasksCount, remainingTasks.Count);
        Assert.Contains("Upload company picture", remainingTasks);
        Assert.Contains("Add company description", remainingTasks);
        Assert.Contains("Post at least 5 jobs", remainingTasks);
        Assert.Contains("Add 2 collaborators", remainingTasks);
        Assert.Contains("Complete mini-game", remainingTasks);
    }

    [Fact]
    public void Calculate_AllTasksCompleted_ReturnsEmptyRemainingTasksList()
    {
        var company = FullyCompleteCompany();

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Empty(remainingTasks);
    }

    [Fact]
    public void Calculate_ProfilePictureMissing_IncludesUploadPictureInRemainingTasks()
    {
        var company = FullyCompleteCompany();
        company.ProfilePicturePath = null;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Contains("Upload company picture", remainingTasks);
    }

    [Fact]
    public void Calculate_AboutUsMissing_IncludesAddDescriptionInRemainingTasks()
    {
        var company = FullyCompleteCompany();
        company.AboutUs = null;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Contains("Add company description", remainingTasks);
    }

    [Fact]
    public void Calculate_PostedJobsBelowMinimum_IncludesPostJobsInRemainingTasks()
    {
        var company = FullyCompleteCompany();
        company.PostedJobsCount = MinimumRequiredPostedJobs - 1;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Contains("Post at least 5 jobs", remainingTasks);
    }

    [Fact]
    public void Calculate_CollaboratorsBelowMinimum_IncludesAddCollaboratorsInRemainingTasks()
    {
        var company = FullyCompleteCompany();
        company.CollaboratorsCount = MinimumRequiredCollaborators - 1;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Contains("Add 2 collaborators", remainingTasks);
    }

    [Fact]
    public void Calculate_MiniGameNotComplete_IncludesCompleteMiniGameInRemainingTasks()
    {
        var company = FullyCompleteCompany();
        company.Game = null;

        var (_, remainingTasks) = profileCompletionCalculator.Calculate(company);

        Assert.Contains("Complete mini-game", remainingTasks);
    }

    // GetSkillsTop3

    [Fact]
    public void GetSkillsTop3_NoJobsForCompany_ReturnsBothListsEmpty()
    {
        jobsRepository.GetAllJobs().Returns(new List<Job>());

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Empty(skillNames);
        Assert.Empty(percentages);
    }

    [Fact]
    public void GetSkillsTop3_JobsFromDifferentCompany_ReturnsBothListsEmpty()
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

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: 1);

        Assert.Empty(skillNames);
        Assert.Empty(percentages);
    }

    [Fact]
    public void GetSkillsTop3_FewerThanThreeSkills_ReturnsOnlyAvailableSkills()
    {
        const int targetCompanyId = 1;
        var job = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 80 },
                new() { Skill = new Skill { SkillName = "SQL" }, RequiredPercentage = 20 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Equal(2, skillNames.Count);
        Assert.Equal(2, percentages.Count);
    }

    [Fact]
    public void GetSkillsTop3_MoreThanThreeSkills_ReturnsOnlyTopThree()
    {
        const int targetCompanyId = 1;
        var job = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },     RequiredPercentage = 40 },
                new() { Skill = new Skill { SkillName = "SQL" },    RequiredPercentage = 30 },
                new() { Skill = new Skill { SkillName = "Azure" },  RequiredPercentage = 20 },
                new() { Skill = new Skill { SkillName = "Docker" }, RequiredPercentage = 10 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Equal(3, skillNames.Count);
        Assert.Equal(3, percentages.Count);
    }

    [Fact]
    public void GetSkillsTop3_SkillsOrderedByWeight_MostRequiredSkillComesFirst()
    {
        const int targetCompanyId = 1;
        var job = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "Docker" }, RequiredPercentage = 10 },
                new() { Skill = new Skill { SkillName = "C#" },     RequiredPercentage = 80 },
                new() { Skill = new Skill { SkillName = "SQL" },    RequiredPercentage = 20 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Equal("C#", skillNames[0]);
        Assert.Equal("SQL", skillNames[1]);
        Assert.Equal("Docker", skillNames[2]);
    }

    [Fact]
    public void GetSkillsTop3_PercentagesRoundToOneHundredTotal()
    {
        const int targetCompanyId = 1;
        var job = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "C#" },  RequiredPercentage = 50 },
                new() { Skill = new Skill { SkillName = "SQL" }, RequiredPercentage = 50 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (_, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Equal(50, percentages[0]);
        Assert.Equal(50, percentages[1]);
    }

    [Fact]
    public void GetSkillsTop3_JobWithNullSkills_SkipsJobGracefully()
    {
        const int targetCompanyId = 1;
        var jobWithNullSkills = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = null,
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { jobWithNullSkills });

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Empty(skillNames);
        Assert.Empty(percentages);
    }

    [Fact]
    public void GetSkillsTop3_SkillWithNullOrEmptyName_IsIgnored()
    {
        const int targetCompanyId = 1;
        var job = new Job
        {
            Company = new Company { CompanyId = targetCompanyId },
            JobSkills = new List<JobSkill>
            {
                new() { Skill = new Skill { SkillName = "" },   RequiredPercentage = 50 },
                new() { Skill = new Skill { SkillName = "C#" }, RequiredPercentage = 50 },
            }
        };
        jobsRepository.GetAllJobs().Returns(new List<Job> { job });

        var (skillNames, _) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Single(skillNames);
        Assert.Equal("C#", skillNames[0]);
    }

    [Fact]
    public void GetSkillsTop3_SameSkillAcrossMultipleJobs_AccumulatesRequiredPercentage()
    {
        const int targetCompanyId = 1;
        var company = new Company { CompanyId = targetCompanyId };
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

        var (skillNames, percentages) = profileCompletionCalculator.GetSkillsTop3(companyId: targetCompanyId);

        Assert.Single(skillNames);
        Assert.Equal("C#", skillNames[0]);
        Assert.Equal(100, percentages[0]);
    }

    // ApplicantsMessage

    [Fact]
    public void ApplicantsMessage_NoPreviousAndNoCurrentApplicants_ReturnsNoApplicantsMessage()
    {
        applicantRepository.GetApplicantsByCompany(1).Returns(new List<Applicant>());

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("No applicants yet. Start posting jobs!", message);
    }

    [Fact]
    public void ApplicantsMessage_NoPreviousApplicantsButSomeThisWeek_ReturnsGreatStartMessage()
    {
        var applicantsThisWeek = new List<Applicant>
        {
            new() { AppliedAt = DateTime.Now.AddDays(-1) },
            new() { AppliedAt = DateTime.Now.AddDays(-3) },
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicantsThisWeek);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        Assert.Equal("Great start! You have 2 new applicants.", message);
    }

    [Fact]
    public void ApplicantsMessage_MoreApplicantsThanLastWeek_ReturnsMoreApplicantsMessage()
    {
        var applicants = new List<Applicant>
        {
            new() { AppliedAt = DateTime.Now.AddDays(-1) },   // this week
            new() { AppliedAt = DateTime.Now.AddDays(-1) },   // this week
            new() { AppliedAt = DateTime.Now.AddDays(-10) },  // previous week
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        // 2 current vs 1 previous = 100% increase
        Assert.Equal("Congrats! You have 100% more applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_FewerApplicantsThanLastWeek_ReturnsFewerApplicantsMessage()
    {
        var applicants = new List<Applicant>
        {
            new() { AppliedAt = DateTime.Now.AddDays(-1) },   // this week
            new() { AppliedAt = DateTime.Now.AddDays(-10) },  // previous week
            new() { AppliedAt = DateTime.Now.AddDays(-11) },  // previous week
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        // 1 current vs 2 previous = 50% decrease
        Assert.Equal("You have 50% fewer applicants than last week.", message);
    }

    [Fact]
    public void ApplicantsMessage_SameNumberAsLastWeek_ReturnsZeroPercentMoreMessage()
    {
        var applicants = new List<Applicant>
        {
            new() { AppliedAt = DateTime.Now.AddDays(-1) },   // this week
            new() { AppliedAt = DateTime.Now.AddDays(-10) },  // previous week
        };
        applicantRepository.GetApplicantsByCompany(1).Returns(applicants);

        var message = profileCompletionCalculator.ApplicantsMessage(companyId: 1);

        // 1 current vs 1 previous = 0% change, falls into the >= 0 branch
        Assert.Equal("Congrats! You have 0% more applicants than last week.", message);
    }
}
