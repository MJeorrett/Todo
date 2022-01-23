using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Dtos;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CreateTodoTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShouldPopulateAllPropertiesWhenRequestIsValid()
    {
        var httpClient = _factory.CreateClient();

        var createResponse = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = "Clean bike",
        });

        await createResponse.AssertIsStatusCode(201);

        var todoId = await createResponse.ReadResponseContentAs<int>();

        var getByIdResponse = await httpClient.GetAsync($"api/todos/{todoId}");

        await getByIdResponse.AssertIsStatusCode(200);

        var todo = await getByIdResponse.ReadResponseContentAs<TodoDetailsDto>();

        Assert.Equal(todoId, todo?.Id);
        Assert.Equal("Clean bike", todo?.Title);
    }
}
