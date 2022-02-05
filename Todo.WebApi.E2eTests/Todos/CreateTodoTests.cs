using NUnit.Framework;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Dtos.Todos;
using Todo.WebApi.E2eTests.Endpoints;
using Todo.WebApi.E2eTests.WebApplicationFactory;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoTests : TestBase
{
    [Test]
    public async Task ShouldReturn401WhenUserNotAuthenticated()
    {
        var httpClient = _factory.CreateClient();

        var response = await httpClient.CreateTodo(new
        {
            title = "not me",
        });

        await response.AssertIsStatusCode(401);
    }

    [Test]
    public async Task ShouldPopulateAllPropertiesWhenRequestIsValid()
    {
        await _factory.CreateAspNetUser("test@mailinator.com", "Sitekit123!");
        var httpClient = await _factory.CreateHttpClientAuthenticatedAsUser("test@mailinator.com", "Sitekit123!");

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
        Assert.AreEqual(_factory.MockedNow.ToDateTimeUtc(), createdTodo!.CreatedAt);
        Assert.Null(createdTodo!.LastUpdatedAt);
    }
}
