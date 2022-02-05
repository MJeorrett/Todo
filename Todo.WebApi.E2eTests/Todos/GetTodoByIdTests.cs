using NUnit.Framework;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos;

public class GetTodoByIdTests : TestBase
{
    [Test]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var authenticatedHttpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

        var createTodoResponse = await authenticatedHttpClient.CreateTodo(new
        {
            title = "Test",
        });

        var createdTodoId = await createTodoResponse.ReadResponseContentAs<int>();

        var unauthenticatedHttpClient = Factory.CreateClient();
        var response = await unauthenticatedHttpClient.GetTodoById(createdTodoId + 1);

        await response.AssertIsStatusCode(401);
    }

    [Test]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

        var createTodoResponse = await httpClient.CreateTodo(new
        {
            title = "Test",
        });

        var createdTodoId = await createTodoResponse.ReadResponseContentAs<int>();

        var response = await httpClient.GetTodoById(createdTodoId + 1);

        await response.AssertIsStatusCode(404);
    }

    [Test]
    public async Task ShouldPopulateAllProperties()
    {
        var httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

        var createResponse = await httpClient.CreateTodo(new
        {
            title = "Clean bike",
        });

        await createResponse.AssertIsStatusCode(201);

        var todoId = await createResponse.ReadResponseContentAs<int>();

        var getByIdResponse = await httpClient.GetTodoById(todoId);

        await getByIdResponse.AssertIsStatusCode(200);

        var createdTodo = await getByIdResponse.ReadResponseContentAs<TodoDetailsDto>();

        Assert.NotNull(createdTodo);
        Assert.AreEqual(todoId, createdTodo!.Id);
        Assert.AreEqual("Clean bike", createdTodo!.Title);
        Assert.AreEqual(Factory.MockedNow.ToDateTimeUtc(), createdTodo!.CreatedAt);
        Assert.Null(createdTodo!.LastUpdatedAt);
    }
}
