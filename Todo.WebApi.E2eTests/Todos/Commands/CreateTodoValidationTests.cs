using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos.Commands;

public class CreateTodoValidationTests : TestBase
{
    private HttpClient _httpClient = null!;

    [OneTimeSetUp]
    public new async Task Initialize()
    {
        _httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");
    }

    [Test]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var actualResult = await _httpClient.CreateTodo(new { });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var actualResult = await _httpClient.CreateTodo(new
        {
            title = "",
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var actualResult = await _httpClient.CreateTodo(new
        {
            title = new string('a', 257),
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn200WhenAllOk()
    {
        var actualResult = await _httpClient.CreateTodo(new
        {
            title = new string('a', 256),
        });

        await actualResult.AssertIsStatusCode(201);
    }
}
