using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Commands;

[Collection("waf")]
public class UpdateTodoTests : TestBase
{
    public UpdateTodoTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    [Fact]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var authenticatedHttpClient = await CreateHttpClientAuthenticatedAsNewUser();

        var existingTodoId = await CreateTodo(authenticatedHttpClient, new
        {
            title = "Feed cat"
        });

        var unauthenticatedHttpClient = Factory.CreateClient();

        var response = await unauthenticatedHttpClient.CallCreateTodo(new
        {
            todoId = existingTodoId,
            title = "Starve cat",
        });

        await response.Should().BeStatusCode(401);
    }

    [Fact]
    public async Task ShouldUpdateAllFields()
    {
        var userId = await Factory.CreateAspNetUser("test@mailinator.com", "Sitekit123!");
        var httpClient = await CreateHttpClientAuthenticatedAsUser("test@mailinator.com", "Sitekit123!");

        var existingTodoId = await CreateTodo(httpClient, new
        {
            title = "Feed cat"
        });

        var response = await httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId,
            title = "Starve cat",
        });

        await response.Should().BeStatusCode(200);

        var updatedTodo = await GetTodoById(httpClient, existingTodoId);

        Assert.Equal(existingTodoId, updatedTodo?.Id);
        Assert.Equal("Starve cat", updatedTodo?.Title);
        Assert.Equal(Factory.MockedNow.ToDateTimeUtc(), updatedTodo?.LastUpdatedAt);
        Assert.Equal(userId, updatedTodo?.LastUpdatedBy);
    }
}
