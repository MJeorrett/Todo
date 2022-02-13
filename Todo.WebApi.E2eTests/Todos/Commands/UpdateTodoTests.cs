using FluentAssertions;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
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

        await response.Should().HaveStatusCode(401);
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

        await response.Should().HaveStatusCode(200);

        var updatedTodo = await GetTodoById(httpClient, existingTodoId);

        var expected = new TodoDetailsDto
        {
            Id = existingTodoId,
            Title = "Starve cat",
            LastUpdatedAt = MockedNowDateTimeUtc,
            LastUpdatedBy = userId,
        };

        updatedTodo.Should().BeEquivalentTo(
            expected,
            options => options.ExcludingCreatedAtAndBy());
    }
}
