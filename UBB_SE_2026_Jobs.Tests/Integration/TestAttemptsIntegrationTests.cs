using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using UBB_SE_2026_Jobs.Library.Domain.Core;
using UBB_SE_2026_Jobs.Library.DTOs;
using UBB_SE_2026_Jobs.Library.Persistence;
using System.Threading.Tasks;
using Xunit;

namespace UBB_SE_2026_Jobs.Tests.Integration;

public class TestAttemptsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TestAttemptsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;

        // Ensure a clean in-memory database for each test instance
        using var scope = _factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<JobsDbContext>();
        database.Database.EnsureDeleted();
        database.Database.EnsureCreated();
    }

    [Fact]
    public async Task PostAttempt_ControllerPersists_ThenCanBeRetrievedByUserAndTest()
    {
        var client = _factory.CreateClient();

        var testAttemptDto = new TestAttemptDto
        {
            TestId = 42,
            ExternalUserId = 123,
            Score = 10m,
            Status = "IN_PROGRESS",
            IsValidated = false
        };

        // Act - POST via controller
        var postResponse = await client.PostAsJsonAsync("api/testattempts", testAttemptDto);
        postResponse.EnsureSuccessStatusCode();

        // Act - GET by user and test 
        var getResponse = await client.GetAsync($"api/testattempts/byuser/{testAttemptDto.ExternalUserId}/bytest/{testAttemptDto.TestId}");
        getResponse.EnsureSuccessStatusCode();

        var returned = await getResponse.Content.ReadFromJsonAsync<TestAttemptDto>();

        // Assert
        Assert.NotNull(returned);
        Assert.Equal(testAttemptDto.TestId, returned!.TestId);
        Assert.Equal(testAttemptDto.ExternalUserId, returned.ExternalUserId);
        Assert.Equal(testAttemptDto.Score, returned.Score);
    }

    [Fact]
    public async Task SeedDatabase_DirectlyThenGetById_ReturnsSeededAttempt()
    {
        int seededId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<JobsDbContext>();


            var test = new Test { Title = "IT Test", Category = "Integration", CreatedAt = System.DateTime.UtcNow };
            db.Tests.Add(test);
            await db.SaveChangesAsync();

            var attempt = new TestAttempt
            {
                TestId = test.Id,
                ExternalUserId = 777,
                Score = 5m,
                Status = "COMPLETED",
            };

            db.TestAttempts.Add(attempt);
            await db.SaveChangesAsync();
            seededId = attempt.Id;
        }

        // Act - fetch via controller by id
        var client = _factory.CreateClient();
        var getResponse = await client.GetAsync($"api/testattempts/{seededId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var dto = await getResponse.Content.ReadFromJsonAsync<TestAttemptDto>();
        Assert.NotNull(dto);
        Assert.Equal(seededId, dto!.Id);
        Assert.Equal(777, dto.ExternalUserId);
    }
}
