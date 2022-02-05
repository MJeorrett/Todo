using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.Infrastructure.Persistence;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Todo.WebApi.E2eTests.Shared.Models;

namespace Todo.WebApi.E2eTests;

public class TestBase
{
    protected CustomWebApplicationFactory Factory = null!;

    private ClientApplicationDetails _testClientApplicationDetails = new();

    [OneTimeSetUp]
    public void Initialize()
    {
        Factory = new CustomWebApplicationFactory();

        var services = Factory.Services.CreateScope().ServiceProvider;
        EnsureDatabasesCreatedAndMigrated(services);
    }

    [SetUp]
    public async Task ResetState()
    {
        await Factory.ResetState();
        await CreateDefaultClientApplication();
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

    private static void EnsureDatabasesCreatedAndMigrated(IServiceProvider services)
    {
        var logger = services.GetRequiredService<ILogger<TestBase>>();

        try
        {
            logger.LogInformation("Migrating database.");

            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();

            logger.LogInformation("Database migration done.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception trying to ensure database created and migrated.");
        }
    }

    private async Task CreateDefaultClientApplication()
    {
        await Factory.CreatePkceClientApplication(
            _testClientApplicationDetails.ClientId,
            _testClientApplicationDetails.Scope,
            _testClientApplicationDetails.RedirectUri);
    }

    public static async Task<int> CreateTodo(HttpClient httpClient, object createTodoRequest)
    {
        var response = await httpClient.CallCreateTodo(createTodoRequest);

        await response.AssertIsStatusCode(201);

        var todoId = await response.ReadResponseContentAs<int>();
        return todoId;
    }

    public static async Task<TodoDetailsDto> GetTodoById(HttpClient httpClient, int todoId)
    {
        var response = await httpClient.CallGetTodoById(todoId);

        await response.AssertIsStatusCode(200);

        var todo = await response.ReadResponseContentAs<TodoDetailsDto>();
        return todo;
    }
}
