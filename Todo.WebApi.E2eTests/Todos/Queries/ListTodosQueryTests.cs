using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
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

        await response.Should().BeStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturnCompletePage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 2);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "B todo" },
        };

        (await response.Should()
            .ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldReturnPartialPage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 3);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "B todo" },
        };

        (await response.Should()
            .ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldReturnMiddlePage()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(2, 1);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "B todo" },
        };

        (await response.Should()
            .ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }

    [Fact]
    public async Task ShouldOrderByTitleByDefault()
    {
        await CreateTodo(_authenticatedHttpClient, new { title = "A todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "C todo" });
        await CreateTodo(_authenticatedHttpClient, new { title = "B todo" });

        var response = await _authenticatedHttpClient.CallListTodos(1, 10);

        var expected = new List<TodoDetailsDto>
        {
            new TodoDetailsDto() { Title = "A todo" },
            new TodoDetailsDto() { Title = "B todo" },
            new TodoDetailsDto() { Title = "C todo" },
        };

        (await response.Should()
            .ContainPaginatedListOf<TodoDetailsDto>())
            .Which.Items.Should().EqualIgnoringIdAndAuditProperties(expected);
    }
}
