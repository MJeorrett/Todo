using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Todo.Infrastructure.Persistence;
using Todo.WebApi.E2eTests.WebApplicationFactory;

namespace Todo.WebApi.E2eTests;

public class TestBase
{
    protected CustomWebApplicationFactory Factory = null!;

    [OneTimeSetUp]
    public async Task Initialize()
    {
        Factory = new CustomWebApplicationFactory();

        var services = Factory.Services.CreateScope().ServiceProvider;
        EnsureDatabasesCreatedAndMigrated(services);

        await Factory.ResetState();
    }

    private static void EnsureDatabasesCreatedAndMigrated(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<TestBase>>();

        try
        {
            logger.LogInformation("Migrating database.");

            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            logger.LogInformation("Database migration done.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception trying to ensure database created and migrated.");
        }
    }
}
