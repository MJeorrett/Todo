using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Todo.Application.UnitTests.Shared;

internal class TestDbContext<TEntity> : DbContext where TEntity : class
{
    public DbSet<TEntity> Entities { get; init; } = null!;

    public TestDbContext(DbContextOptions<TestDbContext<TEntity>> options):
        base(options)
    {

    }

    public static TestDbContext<TEntity> CreateInMemory()
    {
        var dbConnection = new SqliteConnection("Filename=:memory:");
        dbConnection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<TestDbContext<TEntity>>()
            .UseSqlite(dbConnection)
            .Options;

        var dbContext = new TestDbContext<TEntity>(dbContextOptions);

        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    public TestDbContext<TEntity> WithEntity(TEntity entity)
    {
        Entities.Add(entity);
        SaveChanges();

        return this;
    }

    public TestDbContext<TEntity> WithEntities(List<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            Entities.Add(entity);
        }

        SaveChanges();

        return this;
    }
}
