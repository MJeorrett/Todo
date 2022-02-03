using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Todo.Infrastructure.Persistence;

namespace Todo.WebApi.E2eTests;

[SetUpFixture]
public class GlobalSetUp
{
    [OneTimeSetUp]
    public async Task SetUp()
    {
        var factory = new CustomWebApplicationFactory();

        var services = factory.Services.CreateScope().ServiceProvider;
        EnsureDatabasesCreatedAndMigrated(services);

        await factory.ResetState();
    }

    private static void EnsureDatabasesCreatedAndMigrated(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<GlobalSetUp>>();

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
