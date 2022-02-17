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
using Todo.WebApi.E2eTests.Shared.Extensions;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Commands;

[Collection("waf")]
public class UpdateTodoValidationTests : TestBase, IAsyncLifetime
{
    private HttpClient _httpClient = null!;
    private int existingTodoId;

    private UpdateTodoDto BuildValidDto() => new()
    {
        TodoId = existingTodoId,
        Title = "Make awesome app",
        StatusId = 1,
    };

    public UpdateTodoValidationTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _httpClient = await CreateHttpClientAuthenticatedAsNewUser();

        existingTodoId = await _httpClient.DoCreateTodoWithTitle("Play pong");
    }

    [Fact]
    public async Task ShouldReturn200WhenAllOk()
    {
        var actualResult = await _httpClient.CallUpdateTodo(BuildValidDto());

        await actualResult.Should().HaveStatusCode(200);
    }

    [Fact]
    public async Task ShouldReturn404WhenTodoNotFound()
    {
        var response = await _httpClient.CallUpdateTodo(BuildValidDto() with
        {
            TodoId = existingTodoId + 1,
        });

        await response.Should().HaveStatusCode(404);
    }

    [Fact]
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var actualResult = await _httpClient.CallUpdateTodo(BuildValidDto() with
        {
            Title = null,
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var actualResult = await _httpClient.CallUpdateTodo(BuildValidDto() with
        {
            Title = "",
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var actualResult = await _httpClient.CallUpdateTodo(BuildValidDto() with
        {
            Title = new string('a', 257),
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTodoStatusIsNotValid()
    {
        var actualResult = await _httpClient.CallCreateTodo(BuildValidDto() with
        {
            StatusId = (int)Enum.GetValues<TodoStatus>().Max() + 1,
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("StatusId");
    }
}
