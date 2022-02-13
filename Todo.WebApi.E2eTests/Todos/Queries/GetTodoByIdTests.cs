using FluentAssertions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Queries;

[Collection("waf")]
public class GetTodoByIdTests : TestBase, IAsyncLifetime
{
    private HttpClient _authenticatedHttpClient = null!;

    public GetTodoByIdTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
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
        var todoId = await CreateTodo(_authenticatedHttpClient);

        var unauthenticatedHttpClient = Factory.CreateClient();
        var response = await unauthenticatedHttpClient.CallGetTodoById(todoId);

        await response.Should().HaveStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var todoId = await CreateTodo(_authenticatedHttpClient);

        var notATodoId = todoId + 1;

        var response = await _authenticatedHttpClient.CallGetTodoById(notATodoId);

        await response.Should().HaveStatusCode(404);
    }

    private static async Task<int> CreateTodo(HttpClient httpClient)
    {
        var createResponse = await httpClient.CallCreateTodo(new
        {
            title = "Clean bike",
        });

        await createResponse.Should().HaveStatusCode(201);

        var parsedResponse = await createResponse.Content.ReadFromJsonAsync<AppResponse<int>>();

        return parsedResponse!.Content;
    }
}
