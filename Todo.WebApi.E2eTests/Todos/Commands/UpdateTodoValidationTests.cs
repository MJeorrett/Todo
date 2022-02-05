using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos.Commands;

public class UpdateTodoValidationTests : TestBase
{
    private HttpClient _httpClient = null!;
    private int existingTodoId;

    [OneTimeSetUp]
    public async Task BeforeAll()
    {
        _httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");
    }

    [SetUp]
    public async Task BeforeEach()
    {
        existingTodoId = await CreateTodo(_httpClient, new
        {
            title = "Play pong"
        });
    }

    [Test]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var response = await _httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId + 1,
            title = "Play lots of pong"
        });

        await response.AssertIsStatusCode(404);
    }

    [Test]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var actualResult = await _httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var actualResult = await _httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId,
            title = "",
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var actualResult = await _httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId,
            title = new string('a', 257),
        });

        await actualResult.AssertIs400WithErrorForField("Title");
    }

    [Test]
    public async Task ShouldReturn200WhenAllOk()
    {
        var actualResult = await _httpClient.CallUpdateTodo(new
        {
            todoId = existingTodoId,
            title = new string('a', 256),
        });

        await actualResult.AssertIsStatusCode(200);
    }
}
