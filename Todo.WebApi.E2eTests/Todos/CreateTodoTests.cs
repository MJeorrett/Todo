using NUnit.Framework;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Dtos;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoTests
{
    private CustomWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public void Initialize()
    {
        _factory = new CustomWebApplicationFactory();
    }

    [Test]
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

        var actual = await getByIdResponse.ReadResponseContentAs<TodoDetailsDto>();

        Assert.NotNull(actual);
        Assert.AreEqual(todoId, actual!.Id);
        Assert.AreEqual("Clean bike", actual!.Title);
        Assert.AreEqual(_factory.MockedNow.ToDateTimeUtc(), actual!.CreatedAt);
        Assert.Null(actual!.LastUpdatedAt);
    }
}
