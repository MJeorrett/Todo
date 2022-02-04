using NUnit.Framework;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoValidationTests : TestBase
{
    [Test]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {

        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = "",
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var httpClient = _factory.CreateClient();

        var actualResult = await httpClient.PostAsJsonAsync("api/todos", new
        {
            title = new string('a', 257),
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
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
