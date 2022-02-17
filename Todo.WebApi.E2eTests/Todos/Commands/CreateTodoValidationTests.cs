using FluentAssertions;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.Domain.Enums;
using Todo.WebApi.E2eTests.Models;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Commands;

[Collection("waf")]
public class CreateTodoValidationTests : TestBase, IAsyncLifetime
{
    private HttpClient _httpClient = null!;

    private readonly CreateTodoDto _validDto = new()
    {
        Title = "Make awesome app",
        StatusId = 1,
    };

    public CreateTodoValidationTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _httpClient = await CreateHttpClientAuthenticatedAsNewUser();
    }

    [Fact]
    public async Task ShouldReturn200WhenRequestIsValid()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto);

        await actualResult.Should().HaveStatusCode(201);
    }

    [Fact]
    public async Task ShouldReturn200WhenNoStatusIdProvided()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto with
        {
            StatusId = null,
        });

        await actualResult.Should().HaveStatusCode(201);
    }

    [Fact]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto with
        {
            Title = null,
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto with
        {
            Title = "",
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto with
        {
            Title = new string('a', 257),
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTodoStatusIsNotValid()
    {
        var actualResult = await _httpClient.CallCreateTodo(_validDto with
        {
            StatusId = (int)Enum.GetValues<TodoStatus>().Max() + 1,
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("StatusId");
    }
}
