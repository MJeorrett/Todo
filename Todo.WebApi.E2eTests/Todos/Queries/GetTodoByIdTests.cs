using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos.Queries;

public class GetTodoByIdTests : TestBase
{
    private HttpClient authenticatedHttpClient = null!;

    [SetUp]
    public new async Task Initialize()
    {
        authenticatedHttpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");
    }

    [Test]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var todoId = await CreateTodo(authenticatedHttpClient);

        var unauthenticatedHttpClient = Factory.CreateClient();
        var response = await unauthenticatedHttpClient.GetTodoById(todoId);

        await response.AssertIsStatusCode(401);
    }

    [Test]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var todoId = await CreateTodo(authenticatedHttpClient);

        var notATodoId = todoId + 1;

        var response = await authenticatedHttpClient.GetTodoById(notATodoId);

        await response.AssertIsStatusCode(404);
    }

    private static async Task<int> CreateTodo(HttpClient httpClient)
    {
        var createResponse = await httpClient.CreateTodo(new
        {
            title = "Clean bike",
        });

        await createResponse.AssertIsStatusCode(201);

        var todoId = await createResponse.ReadResponseContentAs<int>();
        return todoId;
    }
}
