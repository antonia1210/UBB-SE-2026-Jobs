using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.Services.CompanyService;
using UBB_SE_2026_Jobs.Tests.Fakes;
using UBB_SE_2026_Jobs.Tests.Helpers;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class CompanyServiceTests
{
    private readonly FakeCompanyRepository companyRepository = new();
    private readonly PussyCatsCompanyService service;

    public CompanyServiceTests()
    {
        service = new PussyCatsCompanyService(companyRepository);
    }

    [Fact]
    public async Task AddAsync_ValidCompany_ReturnsCompanyWithAssignedId()
    {
        var company = new CompanyBuilder().WithName("TestCompany").Build();
        company.CompanyId = 0;

        var result = await service.AddAsync(company);

        Assert.NotNull(result);
        Assert.True(result.CompanyId > 0);
        Assert.Equal("TestCompany", result.Name);
    }

    [Fact]
    public async Task AddAsync_ValidCompany_CanBeRetrievedAfterAdding()
    {
        var company = new CompanyBuilder().WithName("TestCompany").WithEmail("test@test.com").Build();
        company.CompanyId = 0;

        var added = await service.AddAsync(company);
        var retrieved = await service.GetByIdAsync(added.CompanyId);

        Assert.NotNull(retrieved);
        Assert.Equal(added.CompanyId, retrieved.CompanyId);
        Assert.Equal("TestCompany", retrieved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingCompany_ReturnsCompany()
    {
        const int companyId = 1;
        companyRepository.Seed(new CompanyBuilder().WithId(companyId).WithName("Google").Build());

        var result = await service.GetByIdAsync(companyId);

        Assert.NotNull(result);
        Assert.Equal(companyId, result.CompanyId);
        Assert.Equal("Google", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingCompany_ReturnsNull()
    {
        const int nonExistentId = 999;

        var result = await service.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_NoCompaniesSeeded_ReturnsEmptyList()
    {
        var result = await service.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleCompaniesSeeded_ReturnsAllCompanies()
    {
        companyRepository.Seed(
            new CompanyBuilder().WithId(1).WithName("Google").Build(),
            new CompanyBuilder().WithId(2).WithName("Microsoft").Build(),
            new CompanyBuilder().WithId(3).WithName("Amazon").Build());

        var result = await service.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCompany_ChangesAreReflectedOnRetrieval()
    {
        const int companyId = 1;
        companyRepository.Seed(new CompanyBuilder().WithId(companyId).WithName("OldName").Build());

        var company = await service.GetByIdAsync(companyId);
        company!.Name = "NewName";
        await service.UpdateAsync(company);

        var updated = await service.GetByIdAsync(companyId);
        Assert.NotNull(updated);
        Assert.Equal("NewName", updated.Name);
    }

    [Fact]
    public async Task RemoveAsync_ExistingCompany_CompanyIsNoLongerRetrievable()
    {
        const int companyId = 5;
        companyRepository.Seed(new CompanyBuilder().WithId(companyId).Build());

        await service.RemoveAsync(companyId);

        var result = await service.GetByIdAsync(companyId);
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ExistingCompany_OtherCompaniesAreUnaffected()
    {
        const int removedId = 5, keptId = 6;
        companyRepository.Seed(
            new CompanyBuilder().WithId(removedId).Build(),
            new CompanyBuilder().WithId(keptId).WithName("Kept").Build());

        await service.RemoveAsync(removedId);

        var remaining = await service.GetAllAsync();
        Assert.Single(remaining);
        Assert.Equal(keptId, remaining[0].CompanyId);
    }

    [Fact]
    public async Task GetAllAsync_AfterAddingCompanies_ReturnsCorrectCount()
    {
        var alpha = new CompanyBuilder().WithName("Alpha").Build();
        alpha.CompanyId = 0;
        var beta = new CompanyBuilder().WithName("Beta").Build();
        beta.CompanyId = 0;

        await service.AddAsync(alpha);
        await service.AddAsync(beta);

        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count);
    }
}
