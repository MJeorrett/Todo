using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos.Commands;

public class UpdateTodoTests : TestBase
{
    [Test]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var authenticatedHttpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

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

        await response.AssertIsStatusCode(401);
    }

    [Test]
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

        await response.AssertIsStatusCode(200);

        var updatedTodo = await GetTodoById(httpClient, existingTodoId);

        Assert.AreEqual(existingTodoId, updatedTodo.Id);
        Assert.AreEqual("Starve cat", updatedTodo.Title);
        Assert.AreEqual(Factory.MockedNow.ToDateTimeUtc(), updatedTodo!.LastUpdatedAt);
        Assert.AreEqual(userId, updatedTodo.LastUpdatedBy);
    }
}
