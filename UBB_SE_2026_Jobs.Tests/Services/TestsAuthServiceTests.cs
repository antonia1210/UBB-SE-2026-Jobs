using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using UBB_SE_2026_Jobs.Library.Domain;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Repositories;
using UBB_SE_2026_Jobs.Library.Repositories.Interfaces;
using UBB_SE_2026_Jobs.Library.Repositories.Users;
using UBB_SE_2026_Jobs.Library.Services;

namespace UBB_SE_2026_Jobs.Tests.Services;

public class TestsAuthServiceTests
{
    private readonly ITestsCompanyRepository companyRepository = Substitute.For<ITestsCompanyRepository>();
    private readonly IUserRepository userRepository = Substitute.For<IUserRepository>();
    private readonly IRecruiterRepository recruiterRepository = Substitute.For<IRecruiterRepository>();
    private readonly TestsAuthService testsAuthService;

    private const string KnownPlaintextPassword = "TestPassword123!";
    private const string JwtSecretKey = "this-is-a-sufficiently-long-secret-key-for-tests";

    public TestsAuthServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TiJwt:Key"] = JwtSecretKey,
            })
            .Build();

        testsAuthService = new TestsAuthService(
            companyRepository,
            userRepository,
            recruiterRepository,
            configuration);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static string HashPassword(User user, string plaintext)
    {
        return new PasswordHasher<User>().HashPassword(user, plaintext);
    }

    private static User UserWithHashedPassword(int userId, string email, string plaintext) 
    {
        var user = new User { Id = userId, Email = email, FirstName = "Test", LastName = "User" };
        user.PasswordHash = HashPassword(user, plaintext);
        return user;
    }

    private static LoginDto LoginWith(string email, string password) =>
        new() { Email = email, Password = password };

    private static RegisterDto RegisterWith(string email, string role, int? companyId = null) =>
        new() { Email = email, Role = role, CompanyId = companyId };

    // -------------------------------------------------------------------------
    // LoginAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task LoginAsync_UserNotFound_ReturnsNull()
    {
        userRepository.GetByEmailAsync("unknown@example.com").Returns((User?)null);

        var result = await testsAuthService.LoginAsync(LoginWith("unknown@example.com", KnownPlaintextPassword));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        var user = UserWithHashedPassword(userId: 1, email: "user@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, "WrongPassword!"));

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsResponseWithUserDetails()
    {
        var user = UserWithHashedPassword(userId: 1, email: "user@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.UserId);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task LoginAsync_CorrectCredentials_ReturnsNonEmptyToken()
    {
        var user = UserWithHashedPassword(userId: 1, email: "user@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.Token));
    }

    [Fact]
    public async Task LoginAsync_UserIsRecruiter_ReturnsRecruiterRole()
    {
        var user = UserWithHashedPassword(userId: 1, email: "recruiter@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns(42);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.Equal("Recruiter", result!.Role);
    }

    [Fact]
    public async Task LoginAsync_UserIsNotRecruiter_ReturnsCandidateRole()
    {
        var user = UserWithHashedPassword(userId: 1, email: "candidate@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.Equal("Candidate", result!.Role);
    }

    [Fact]
    public async Task LoginAsync_UserIsRecruiter_ReturnsCompanyId()
    {
        const int companyId = 42;
        var user = UserWithHashedPassword(userId: 1, email: "recruiter@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns(companyId);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.Equal(companyId, result!.CompanyId);
    }

    [Fact]
    public async Task LoginAsync_UserIsCandidate_ReturnsNullCompanyId()
    {
        var user = UserWithHashedPassword(userId: 1, email: "candidate@example.com", KnownPlaintextPassword);
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        var result = await testsAuthService.LoginAsync(LoginWith(user.Email, KnownPlaintextPassword));

        Assert.NotNull(result);
        Assert.Null(result!.CompanyId);
    }

    // -------------------------------------------------------------------------
    // RegisterAsync
    // -------------------------------------------------------------------------

    [Fact]
    public async Task RegisterAsync_UserNotFound_ReturnsNull()
    {
        userRepository.GetByEmailAsync("unknown@example.com").Returns((User?)null);

        var result = await testsAuthService.RegisterAsync(RegisterWith("unknown@example.com", role: "Candidate"));

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_CandidateRole_ReturnsResponseWithCandidateRole()
    {
        var user = new User { Id = 1, Email = "candidate@example.com", FirstName = "Candidate", LastName = "User" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Candidate"));

        Assert.NotNull(result);
        Assert.Equal("Candidate", result!.Role);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Name, result.Name);
    }

    [Fact]
    public async Task RegisterAsync_CandidateRole_ReturnsNullCompanyId()
    {
        var user = new User { Id = 1, Email = "candidate@example.com", FirstName = "Candidate", LastName = "User" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Candidate"));

        Assert.NotNull(result);
        Assert.Null(result!.CompanyId);
    }

    [Fact]
    public async Task RegisterAsync_RecruiterRoleWithNoCompanyId_ReturnsNull()
    {
        var user = new User { Id = 1, Email = "recruiter@example.com", FirstName = "Recruiter", LastName = "User" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Recruiter", companyId: null));

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_RecruiterRoleWithInvalidCompanyId_ReturnsNull()
    {
        const int nonExistentCompanyId = 999;
        var user = new User { Id = 1, Email = "recruiter@example.com", FirstName = "Recruiter", LastName = "User" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        companyRepository.GetById(nonExistentCompanyId).Returns((Company?)null);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Recruiter", companyId: nonExistentCompanyId));

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_RecruiterRoleWithValidCompany_ReturnsResponseWithRecruiterRoleAndCompanyId()
    {
        const int companyId = 10;
        var user = new User { Id = 1, Email = "recruiter@example.com", FirstName = "Recruiter", LastName = "User" };
        var company = new Company { CompanyId = companyId, Name = "Acme Corp" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        companyRepository.GetById(companyId).Returns(company);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Recruiter", companyId: companyId));

        Assert.NotNull(result);
        Assert.Equal("Recruiter", result!.Role);
        Assert.Equal(companyId, result.CompanyId);
    }

    [Fact]
    public async Task RegisterAsync_RecruiterNotYetRegistered_AddsRecruiterRecord()
    {
        const int companyId = 10;
        var user = new User { Id = 1, Email = "recruiter@example.com", FirstName = "Recruiter", LastName = "User" };
        var company = new Company { CompanyId = companyId, Name = "Acme Corp" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        companyRepository.GetById(companyId).Returns(company);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns((int?)null);

        await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Recruiter", companyId: companyId));

        await recruiterRepository.Received(1).AddAsync(Arg.Is<Recruiter>(recruiter =>
            recruiter.UserId == user.Id && recruiter.CompanyId == companyId));
    }

    [Fact]
    public async Task RegisterAsync_RecruiterAlreadyRegistered_DoesNotAddDuplicateRecruiterRecord()
    {
        const int companyId = 10;
        var user = new User { Id = 1, Email = "recruiter@example.com", FirstName = "Recruiter", LastName = "User" };
        var company = new Company { CompanyId = companyId, Name = "Acme Corp" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);
        companyRepository.GetById(companyId).Returns(company);
        recruiterRepository.GetCompanyIdForUserAsync(user.Id).Returns(companyId);

        await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Recruiter", companyId: companyId));

        await recruiterRepository.DidNotReceive().AddAsync(Arg.Any<Recruiter>());
    }

    [Fact]
    public async Task RegisterAsync_AnyValidRegistration_ReturnsNonEmptyToken()
    {
        var user = new User { Id = 1, Email = "candidate@example.com", FirstName = "Candidate", LastName = "User" };
        userRepository.GetByEmailAsync(user.Email).Returns(user);

        var result = await testsAuthService.RegisterAsync(RegisterWith(user.Email, role: "Candidate"));

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.Token));
    }
}
