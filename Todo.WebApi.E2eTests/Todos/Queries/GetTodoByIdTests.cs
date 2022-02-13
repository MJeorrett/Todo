using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
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
        var todoId = await _authenticatedHttpClient.DoCreateTodoWithTitle("Clean bike");

        var unauthenticatedHttpClient = Factory.CreateClient();
        var response = await unauthenticatedHttpClient.CallGetTodoById(todoId);

        await response.Should().HaveStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var todoId = await _authenticatedHttpClient.DoCreateTodoWithTitle("Clean bike");

        var notATodoId = todoId + 1;

        var response = await _authenticatedHttpClient.CallGetTodoById(notATodoId);

        await response.Should().HaveStatusCode(404);
    }
}
