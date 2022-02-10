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
    private static int TestMapper(TestEntity entity) => entity.Id;

    [Fact]
    public async Task ShouldThrowExceptionWhenPageNumberIsLessThan1()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
            {
                PageNumber = 0,
                PageSize = 1,
            }, TestMapper));
    }

    [Fact]
    public async Task ShouldThrowExceptionWhenPageSizeIsLessThan1()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
            {
                PageNumber = 1,
                PageSize = 0,
            }, TestMapper));
    }

    [Fact]
    public async Task ShouldReturnNoItemsWhenNoItemsInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory();

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 1,
        }, TestMapper);

        Assert.Equal(1, actual.PageNumber);
        Assert.Equal(1, actual.PageSize);
        Assert.Equal(0, actual.TotalPages);
        Assert.Equal(0, actual.TotalCount);
        Assert.False(actual.HasPreviousPage);
        Assert.False(actual.HasNextPage);
        Assert.Equal(new List<int> { }, actual.Items);
    }

    [Fact]
    public async Task ShouldReturn1ItemWhenPageSize1AndPageNumber1AndSingleItemInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntity(new(1));

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 1,
        }, TestMapper);

        Assert.Equal(1, actual.PageNumber);
        Assert.Equal(1, actual.PageSize);
        Assert.Equal(1, actual.TotalPages);
        Assert.Equal(1, actual.TotalCount);
        Assert.False(actual.HasPreviousPage);
        Assert.False(actual.HasNextPage);
        Assert.Equal(new List<int> { 1 }, actual.Items);
    }

    [Fact]
    public async Task ShouldReturnHasNextPageTrueWhenMoreItemsExistThanReturned()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 1,
        }, TestMapper);

        Assert.True(actual.HasNextPage);
    }

    [Fact]
    public async Task ShouldReturnHasNextPageFalseWhenNoMoreItemsExistThanReturned()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 3,
        }, TestMapper);

        Assert.False(actual.HasNextPage);
    }

    [Fact]
    public async Task ShouldReturnHasPreviousPageTrueWhenResultsDontStartFromFirstItemInSource()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 2,
            PageSize = 1,
        }, TestMapper);

        Assert.True(actual.HasPreviousPage);
    }

    [Fact]
    public async Task ShouldReturnTotalPagesCorrectlyWhenCountIsExactMultipleOfPageSize()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3), new(4) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 2,
        }, TestMapper);

        Assert.Equal(2, actual.TotalPages);
    }

    [Fact]
    public async Task ShouldReturnTotalPagesCorrectlyWhenCountIsNotExactMultipleOfPageSize()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 1,
            PageSize = 2,
        }, TestMapper);

        Assert.Equal(2, actual.TotalPages);
    }

    [Fact]
    public async Task ShouldFloorPageNumberWhenPageXPageSizeIsGreaterThanCountOfItems()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 5,
            PageSize = 2,
        }, TestMapper);

        Assert.Equal(2, actual.PageNumber);
    }

    [Fact]
    public async Task ShouldReturnCorrectItemsWhenMiddelPageRequested()
    {
        using var dbContext = TestDbContext<TestEntity>.CreateInMemory()
            .WithEntities(new() { new(1), new(2), new(3) });

        var actual = await PaginatedListResponse<int>.CreateAsync(dbContext.Entities, new PaginatedListQuery()
        {
            PageNumber = 2,
            PageSize = 1,
        }, TestMapper);

        Assert.Equal(new List<int> { 2 }, actual.Items);
    }
}
