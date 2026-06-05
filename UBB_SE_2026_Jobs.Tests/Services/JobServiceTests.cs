using UBB_SE_2026_Jobs.Library.Services.Jobs;
using UBB_SE_2026_Jobs.Tests.Fakes;
using UBB_SE_2026_Jobs.Tests.Helpers;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class JobServiceTests
{
    private readonly FakeJobRepository jobRepository = new();
    private readonly PussyCatsJobService service;

    public JobServiceTests()
    {
        service = new PussyCatsJobService(jobRepository);
    }

    [Fact]
    public async Task AddAsync_ValidJob_ReturnsJobWithAssignedId()
    {
        var job = new JobBuilder().WithTitle("Backend Dev").Build();
        job.JobId = 0;

        var result = await service.AddAsync(job);

        Assert.NotNull(result);
        Assert.True(result.JobId > 0);
        Assert.Equal("Backend Dev", result.JobTitle);
    }

    [Fact]
    public async Task AddAsync_ValidJob_CanBeRetrievedAfterAdding()
    {
        var job = new JobBuilder().WithTitle("Frontend Dev").Build();
        job.JobId = 0;

        var added = await service.AddAsync(job);
        var retrieved = await service.GetByIdAsync(added.JobId);

        Assert.NotNull(retrieved);
        Assert.Equal(added.JobId, retrieved.JobId);
        Assert.Equal("Frontend Dev", retrieved.JobTitle);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingJob_ReturnsJob()
    {
        const int jobId = 10;
        jobRepository.Seed(new JobBuilder().WithId(jobId).WithTitle("DevOps Engineer").Build());

        var result = await service.GetByIdAsync(jobId);

        Assert.NotNull(result);
        Assert.Equal(jobId, result.JobId);
        Assert.Equal("DevOps Engineer", result.JobTitle);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingJob_ReturnsNull()
    {
        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_NoJobsSeeded_ReturnsEmptyList()
    {
        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleJobsSeeded_ReturnsAllJobs()
    {
        jobRepository.Seed(
            new JobBuilder().WithId(1).WithTitle("Job A").Build(),
            new JobBuilder().WithId(2).WithTitle("Job B").Build(),
            new JobBuilder().WithId(3).WithTitle("Job C").Build());

        var result = await service.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_JobsBelongToCompany_ReturnsOnlyMatchingJobs()
    {
        const int targetCompanyId = 5, otherCompanyId = 9;
        jobRepository.Seed(
            new JobBuilder().WithId(1).WithCompanyId(targetCompanyId).WithTitle("Job A").Build(),
            new JobBuilder().WithId(2).WithCompanyId(targetCompanyId).WithTitle("Job B").Build(),
            new JobBuilder().WithId(3).WithCompanyId(otherCompanyId).WithTitle("Job C").Build());

        var result = await service.GetByCompanyIdAsync(targetCompanyId);

        Assert.Equal(2, result.Count);
        Assert.All(result, j => Assert.Equal(targetCompanyId, j.Company.CompanyId));
    }

    [Fact]
    public async Task GetByCompanyIdAsync_NoJobsForCompany_ReturnsEmptyList()
    {
        jobRepository.Seed(new JobBuilder().WithId(1).WithCompanyId(1).Build());

        var result = await service.GetByCompanyIdAsync(99);

        Assert.Empty(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingJob_ChangesAreReflectedOnRetrieval()
    {
        const int jobId = 1;
        jobRepository.Seed(new JobBuilder().WithId(jobId).WithTitle("Old Title").Build());

        var job = await service.GetByIdAsync(jobId);
        job!.JobTitle = "New Title";
        await service.UpdateAsync(job);

        var updated = await service.GetByIdAsync(jobId);
        Assert.NotNull(updated);
        Assert.Equal("New Title", updated.JobTitle);
    }

    [Fact]
    public async Task RemoveAsync_ExistingJob_JobIsNoLongerRetrievable()
    {
        const int jobId = 7;
        jobRepository.Seed(new JobBuilder().WithId(jobId).Build());

        await service.RemoveAsync(jobId);

        var result = await service.GetByIdAsync(jobId);
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ExistingJob_OtherJobsAreUnaffected()
    {
        const int removedId = 1, keptId = 2;
        jobRepository.Seed(
            new JobBuilder().WithId(removedId).WithTitle("Removed").Build(),
            new JobBuilder().WithId(keptId).WithTitle("Kept").Build());

        await service.RemoveAsync(removedId);

        var remaining = await service.GetAllAsync();
        Assert.Single(remaining);
        Assert.Equal(keptId, remaining[0].JobId);
    }

    [Fact]
    public async Task GetAllAsync_AfterAddingJobs_ReturnsCorrectCount()
    {
        var jobA = new JobBuilder().WithTitle("Job A").Build();
        jobA.JobId = 0;
        var jobB = new JobBuilder().WithTitle("Job B").Build();
        jobB.JobId = 0;

        await service.AddAsync(jobA);
        await service.AddAsync(jobB);

        var result = await service.GetAllAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_AfterRemovingJob_DoesNotIncludeRemovedJob()
    {
        const int companyId = 3;
        jobRepository.Seed(
            new JobBuilder().WithId(1).WithCompanyId(companyId).WithTitle("Stay").Build(),
            new JobBuilder().WithId(2).WithCompanyId(companyId).WithTitle("Go").Build());

        await service.RemoveAsync(2);

        var result = await service.GetByCompanyIdAsync(companyId);
        Assert.Single(result);
        Assert.Equal("Stay", result[0].JobTitle);
    }
}
