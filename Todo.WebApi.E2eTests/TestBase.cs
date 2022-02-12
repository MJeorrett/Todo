using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Todo.WebApi.E2eTests.Shared.Models;
using Xunit;

namespace Todo.WebApi.E2eTests;

public class TestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory = null!;

    private readonly ClientApplicationDetails _testClientApplicationDetails = new();

    public TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    public virtual async Task InitializeAsync()
    {
        await Factory.ResetState();
        await CreateDefaultClientApplication();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<HttpClient> CreateHttpClientAuthenticatedAsUser(string userName, string password)
    {
        return await Factory.CreateHttpClientAuthenticatedAsUser(_testClientApplicationDetails, userName, password);
    }

    public async Task<HttpClient> CreateUserAndAuthenticatedHttpClient(
        string userName,
        string password)
    {
        await Factory.CreateAspNetUser(userName, password);

        return await Factory.CreateHttpClientAuthenticatedAsUser(_testClientApplicationDetails, userName, password);
    }

    public static async Task<int> CreateTodo(HttpClient httpClient, object createTodoRequest)
    {
        var response = await httpClient.CallCreateTodo(createTodoRequest);

        await response.Should().BeStatusCode(201);

        var todoId = await response.ReadResponseContentAs<int>();
        return todoId;
    }

    public static async Task<TodoDetailsDto?> GetTodoById(HttpClient httpClient, int todoId)
    {
        var response = await httpClient.CallGetTodoById(todoId);

        await response.Should().BeStatusCode(200);

        var todo = await response.ReadResponseContentAs<TodoDetailsDto>();

        return todo;
    }

    private async Task CreateDefaultClientApplication()
    {
        await Factory.CreatePkceClientApplication(
            _testClientApplicationDetails.ClientId,
            _testClientApplicationDetails.Scope,
            _testClientApplicationDetails.RedirectUri);
    }
}
