using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos;

public class TodoE2eTests : TestBase
{
    [Test]
    public async Task ShouldPopulateAllProperties()
    {
        var httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

        var todoId = await CreateTodo(httpClient, new
        {
            title = "Clean bike",
        });

        var getByIdResponse = await httpClient.GetTodoById(todoId);

        await getByIdResponse.AssertIsStatusCode(200);

        var createdTodo = await getByIdResponse.ReadResponseContentAs<TodoDetailsDto>();

        Assert.NotNull(createdTodo);
        Assert.AreEqual(todoId, createdTodo!.Id);
        Assert.AreEqual("Clean bike", createdTodo!.Title);
        Assert.AreEqual(Factory.MockedNow.ToDateTimeUtc(), createdTodo!.CreatedAt);
        Assert.Null(createdTodo!.LastUpdatedAt);
    }

    private static async Task<int> CreateTodo(HttpClient httpClient, object createTodoRequest)
    {
        var createResponse = await httpClient.CreateTodo(createTodoRequest);

        await createResponse.AssertIsStatusCode(201);

        var todoId = await createResponse.ReadResponseContentAs<int>();
        return todoId;
    }
}
