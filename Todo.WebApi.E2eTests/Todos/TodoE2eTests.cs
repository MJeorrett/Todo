using NUnit.Framework;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos;

public class TodoE2eTests : TestBase
{
    [Test]
    public async Task ShouldPopulateAllProperties()
    {
        var userId = await Factory.CreateAspNetUser("test@mailinator.com", "Sitekit123!");
        var httpClient = await CreateHttpClientAuthenticatedAsUser("test@mailinator.com", "Sitekit123!");

        var todoId = await CreateTodo(httpClient, new
        {
            title = "Clean bike",
        });

        var getByIdResponse = await httpClient.CallGetTodoById(todoId);

        await getByIdResponse.AssertIsStatusCode(200);

        var createdTodo = await getByIdResponse.ReadResponseContentAs<TodoDetailsDto>();

        Assert.NotNull(createdTodo);
        Assert.AreEqual(todoId, createdTodo!.Id);
        Assert.AreEqual("Clean bike", createdTodo!.Title);
        Assert.AreEqual(Factory.MockedNow.ToDateTimeUtc(), createdTodo!.CreatedAt);
        Assert.AreEqual(userId, createdTodo.CreatedBy);
        Assert.Null(createdTodo!.LastUpdatedAt);
        Assert.IsEmpty(createdTodo.LastUpdatedBy);
    }
}
