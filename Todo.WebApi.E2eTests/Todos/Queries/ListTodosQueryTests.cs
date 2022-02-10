using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos;
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

        _authenticatedHttpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");
    }

    [Fact]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var httpClient = Factory.CreateClient();

        var response = await httpClient.CallListTodos(1, 10);

        await response.AssertIsStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturnCompletePage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 2);

        await response.AssertIsStatusCode(200);

        var actual = await response.ReadResponseContentAs<PaginatedListResponse<TodoDetailsDto>>();

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "B todo" },
        };

        actual!.Items.Should().BeEquivalentToIgnoringIdAndAudit(expected);
    }

    [Fact]
    public async Task ShouldReturnPartialPage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 3);

        await response.AssertIsStatusCode(200);

        var actual = await response.ReadResponseContentAs<PaginatedListResponse<TodoDetailsDto>>();

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "C todo" },
        };

        actual!.Items.Should().BeEquivalentToIgnoringIdAndAudit(expected);
    }

    [Fact]
    public async Task ShouldReturnMiddlePage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(2, 1);

        await response.AssertIsStatusCode(200);

        var actual = await response.ReadResponseContentAs<PaginatedListResponse<TodoDetailsDto>>();

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "B todo" },
        };

        actual!.Items.Should().BeEquivalentToIgnoringIdAndAudit(expected);
    }

    [Fact]
    public async Task ShouldOrderByTitleByDefault()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 10);

        await response.AssertIsStatusCode(200);

        var actual = await response.ReadResponseContentAs<PaginatedListResponse<TodoDetailsDto>>();

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "B todo" },
            new TodoDetailsDto() { Title = "C todo" },
        };

        actual!.Items.Should().BeEquivalentToIgnoringIdAndAudit(expected);
    }
}
