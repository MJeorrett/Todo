using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Todo.Application.Common.Interfaces;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.UnitTests.Shared;

internal static class DbContextFactory
{
    
    public static ApplicationDbContext CreateInMemoryApplicationDbContext()
    {
        var dbConnection = new SqliteConnection("Filename=:memory:");
        dbConnection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(dbConnection)
            .Options;

        var dbContext = new ApplicationDbContext(dbContextOptions, Mock.Of<IDateTimeService>(), Mock.Of<ICurrentUserService>());

        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
