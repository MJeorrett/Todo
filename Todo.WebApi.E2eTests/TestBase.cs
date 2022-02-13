using FluentAssertions;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Models;
using Xunit;

namespace Todo.WebApi.E2eTests;

public class TestBase : IAsyncLifetime
{
    protected readonly CustomWebApplicationFactory Factory = null!;
    protected DateTime MockedNowDateTimeUtc => Factory.MockedNow.ToDateTimeUtc();

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

    public async Task<HttpClient> CreateHttpClientAuthenticatedAsNewUser(string userName = "default-test-user@mailinator.com", string password = "Sitekit123!")
    {
        await Factory.CreateAspNetUser(userName, password);

        return await CreateHttpClientAuthenticatedAsUser(userName, password);
    }

    public static async Task<int> CreateTodo(HttpClient httpClient, object createTodoRequest)
    {
        var response = await httpClient.CallCreateTodo(createTodoRequest);

        await response.Should().HaveStatusCode(201);

        var parsedResponse = await response.Content.ReadFromJsonAsync<AppResponse<int>>();

        return parsedResponse!.Content;
    }

    public static async Task<TodoDetailsDto?> GetTodoById(HttpClient httpClient, int todoId)
    {
        var response = await httpClient.CallGetTodoById(todoId);

        await response.Should().HaveStatusCode(200);

        var parsedResponse = await response.Content.ReadFromJsonAsync<AppResponse<TodoDetailsDto>>();

        return parsedResponse!.Content;
    }

    private async Task CreateDefaultClientApplication()
    {
        await Factory.CreatePkceClientApplication(
            _testClientApplicationDetails.ClientId,
            _testClientApplicationDetails.Scope,
            _testClientApplicationDetails.RedirectUri);
    }
}
