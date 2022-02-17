using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Enums;
using Todo.Domain.Extensions;
using Todo.Infrastructure.Exceptions;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.UnitTests.Shared;
using Xunit;

namespace Todo.Infrastructure.UnitTests.Persistence;

using static Todo.Infrastructure.UnitTests.Shared.MockBuilders.ServiceScopeFactoryMockFactory;

public class EnumInitializerTests
{
    [Fact]
    public async Task ShouldCreateEntityWhenItDoesntExist()
    {
        var dbContext = DbContextFactory.CreateInMemoryApplicationDbContext();
        var mockServiceScopeFactory = BuildServiceScopeFactoryContainingRequiredService(dbContext);

        var underTest = new EnumInitializer<TodoStatus, TodoStatusEntity>(BuildMockLogger(), mockServiceScopeFactory.Object);

        await underTest.StartAsync(CancellationToken.None);

        var createdEntities = await dbContext.TodoStatuses.ToListAsync();

        Assert.Equal(4, createdEntities.Count);
    }

    [Fact]
    public async Task ShouldNotCreateEntityWhenItAlreadyExists()
    {
        var dbContext = DbContextFactory.CreateInMemoryApplicationDbContext();
        var mockServiceScopeFactory = BuildServiceScopeFactoryContainingRequiredService(dbContext);

        dbContext.TodoStatuses.Add(new TodoStatusEntity
        {
            Id = TodoStatus.InProgress,
            Name = TodoStatus.InProgress.GetUserFriendlyName(),
        });

        dbContext.SaveChanges();

        var underTest = new EnumInitializer<TodoStatus, TodoStatusEntity>(BuildMockLogger(), mockServiceScopeFactory.Object);

        await underTest.StartAsync(CancellationToken.None);

        var createdEntities = await dbContext.TodoStatuses.ToListAsync();

        Assert.Equal(4, createdEntities.Count);
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenEntityWithSameIdButDifferentNameExists()
    {
        var dbContext = DbContextFactory.CreateInMemoryApplicationDbContext();
        var mockServiceScopeFactory = BuildServiceScopeFactoryContainingRequiredService(dbContext);

        dbContext.TodoStatuses.Add(new TodoStatusEntity
        {
            Id = TodoStatus.InProgress,
            Name = TodoStatus.InProgress.GetUserFriendlyName() + "a",
        });

        dbContext.SaveChanges();

        var underTest = new EnumInitializer<TodoStatus, TodoStatusEntity>(BuildMockLogger(), mockServiceScopeFactory.Object);

        await Assert.ThrowsAsync<EnumInitializationException>(() =>
            underTest.StartAsync(CancellationToken.None));
    }

    private static ILogger<EnumInitializer<TodoStatus, TodoStatusEntity>> BuildMockLogger()
    {
        return Mock.Of<ILogger<EnumInitializer<TodoStatus, TodoStatusEntity>>>();
    }
}
