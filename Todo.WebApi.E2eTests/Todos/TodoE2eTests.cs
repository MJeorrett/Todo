using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos;

[Collection("waf")]
public class TodoE2eTests : TestBase
{
    public TodoE2eTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    [Fact]
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
        Assert.Equal(todoId, createdTodo?.Id);
        Assert.Equal("Clean bike", createdTodo?.Title);
        Assert.Equal(Factory.MockedNow.ToDateTimeUtc(), createdTodo!.CreatedAt);
        Assert.Equal(userId, createdTodo?.CreatedBy);
        Assert.Null(createdTodo?.LastUpdatedAt);
        Assert.Empty(createdTodo?.LastUpdatedBy);
    }
}
