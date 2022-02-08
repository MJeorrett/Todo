using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.Application.UnitTests.Shared;
using Xunit;

namespace Todo.Application.UnitTests.Common.AppRequests;

public class PaginatedListResponseTests
{
    public record TestEntity(int Id)
    {
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenPageNumberIsLessThan1()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 0));
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenPageSizeIsLessThan1()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 0));
    }

    [Fact]
    public async Task ShouldReturnNoItemsWhenNoItemsInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory();

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 1);

        Assert.Equal(1, actual.PageNumber);
        Assert.Equal(1, actual.PageSize);
        Assert.Equal(0, actual.TotalPages);
        Assert.Equal(0, actual.TotalCount);
        Assert.False(actual.HasPreviousPage);
        Assert.False(actual.HasNextPage);
        Assert.Equal(new List<TestEntity> { }, actual.Items);
    }

    [Fact]
    public async Task ShouldReturn1ItemWhenPageSize1AndPageNumber1AndSingleItemInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 1);

        Assert.Equal(1, actual.PageNumber);
        Assert.Equal(1, actual.PageSize);
        Assert.Equal(1, actual.TotalPages);
        Assert.Equal(1, actual.TotalCount);
        Assert.False(actual.HasPreviousPage);
        Assert.False(actual.HasNextPage);
        Assert.Equal(new List<TestEntity> { new(1) }, actual.Items);
    }

    [Fact]
    public async Task ShouldReturnHasNextPageTrueWhenMoreItemsExistThanReturned()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 1);

        Assert.True(actual.HasNextPage);
    }

    [Fact]
    public async Task ShouldReturnHasNextPageFalseWhenNoMoreItemsExistThanReturned()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 3);

        Assert.False(actual.HasNextPage);
    }

    [Fact]
    public async Task ShouldReturnHasPreviousPageTrueWhenResultsDontStartFromFirstItemInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 2, 1);

        Assert.True(actual.HasPreviousPage);
    }

    [Fact]
    public async Task ShouldReturnTotalPagesCorrectlyWhenCountIsExactMultipleOfPageSize()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3), new(4) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 2);

        Assert.Equal(2, actual.TotalPages);
    }

    [Fact]
    public async Task ShouldReturnTotalPagesCorrectlyWhenCountIsNotExactMultipleOfPageSize()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 1, 2);

        Assert.Equal(2, actual.TotalPages);
    }

    [Fact]
    public async Task ShouldFloorPageNumberWhenPageXPageSizeIsGreaterThanCountOfItems()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 5, 2);

        Assert.Equal(2, actual.PageNumber);
    }

    [Fact]
    public async Task ShouldReturnCorrectItemsWhenMiddelPageRequested()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<TestEntity>.CreateAsync(dbContext.Entities, 2, 1);

        Assert.Equal(new List<TestEntity> { new(2) }, actual.Items);
    }
}
