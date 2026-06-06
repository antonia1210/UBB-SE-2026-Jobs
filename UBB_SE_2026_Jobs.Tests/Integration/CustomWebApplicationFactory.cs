using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UBB_SE_2026_Jobs.Api;
using UBB_SE_2026_Jobs.Library.Persistence;

namespace UBB_SE_2026_Jobs.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<PartialProgram>
{
    private readonly string _inMemoryDatabaseName = $"IntegrationTestsDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment("IntegrationTests");

        webHostBuilder.ConfigureServices(services =>
        {
            var registrationsToRemove = services
                .Where(descriptor =>
                    descriptor.ServiceType == typeof(DbContextOptions<JobsDbContext>)
                    || descriptor.ServiceType == typeof(JobsDbContext)
                    || (descriptor.ImplementationType != null && descriptor.ImplementationType.Name.Contains("JobsDbContext")))
                .ToList();

            foreach (var descriptor in registrationsToRemove)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<JobsDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemoryDatabaseName);
            });

            var temporaryProvider = services.BuildServiceProvider();
            using var serviceScope = temporaryProvider.CreateScope();
            var jobsDbContext = serviceScope.ServiceProvider.GetRequiredService<JobsDbContext>();
            jobsDbContext.Database.EnsureDeleted();
            jobsDbContext.Database.EnsureCreated();
        });
    }
}