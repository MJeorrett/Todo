using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.WebApi.E2eTests.Models;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Queries;

[Collection("waf")]
public class ListTodosQueryTests : TestBase
{
    private HttpClient _authenticatedHttpClient = null!;

    public ListTodosQueryTests(WebApplicationFixture fixture) :
        base(fixture.Factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _authenticatedHttpClient = await CreateHttpClientAuthenticatedAsNewUser();
    }

    [Fact]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var httpClient = Factory.CreateClient();

        var response = await httpClient.CallListTodos(1, 10);

        await response.Should().HaveStatusCode(401);
    }

    [Fact]
    public async Task ShouldSuccessfullyReturnWhenNoTodosExist()
    {
        var response = await _authenticatedHttpClient.CallListTodos(1, 10);

        var expected = new PaginatedListResponse<TodoDetailsDto>(new() { }, 0, 0, 1, 10);

        await response.Should().HaveStatusCode(200);
        (await response.Should().ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ShouldReturnCompletePage()
    {
        await _authenticatedHttpClient.DoCreateTodosWithTitles("A todo", "C todo", "B todo");

        var response = await _authenticatedHttpClient.CallListTodos(1, 2);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo", StatusId = 0, StatusName = "New" },
            new TodoDetailsDto() { Title = "B todo", StatusId = 0, StatusName = "New" },
        };

        await response.Should().HaveStatusCode(200);
        (await response.Should().ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldReturnPartialPage()
    {
        await _authenticatedHttpClient.DoCreateTodosWithTitles("A todo", "B todo");

        var response = await _authenticatedHttpClient.CallListTodos(1, 3);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo", StatusId = 0, StatusName = "New" },
            new TodoDetailsDto() { Title = "B todo", StatusId = 0, StatusName = "New" },
        };

        await response.Should().HaveStatusCode(200);
        (await response.Should().ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldReturnMiddlePage()
    {
        await _authenticatedHttpClient.DoCreateTodosWithTitles("A todo", "C todo", "B todo");

        var response = await _authenticatedHttpClient.CallListTodos(2, 1);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "B todo", StatusId = 0, StatusName = "New" },
        };

        await response.Should().HaveStatusCode(200);
        (await response.Should().ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldOrderByTitleByDefault()
    {
        await _authenticatedHttpClient.DoCreateTodosWithTitles("A todo", "C todo", "B todo");

        var response = await _authenticatedHttpClient.CallListTodos(1, 10);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo", StatusId = 0, StatusName = "New" },
            new TodoDetailsDto() { Title = "B todo", StatusId = 0, StatusName = "New" },
            new TodoDetailsDto() { Title = "C todo", StatusId = 0, StatusName = "New" },
        };

        await response.Should().HaveStatusCode(200);
        (await response.Should().ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }
}
