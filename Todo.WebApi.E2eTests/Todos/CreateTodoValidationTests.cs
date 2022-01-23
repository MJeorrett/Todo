using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CreateTodoValidationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {

        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = "",
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = new string('a', 257),
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn200WhenAllOk()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = new string('a', 256),
        });

        await actualResult.AssertIsStatusCode(201);
    }
}
