using FluentAssertions;
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
public class CreateTodoTests : TestBase
{
    public CreateTodoTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {

    }

    [Fact]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var httpClient = Factory.CreateClient();

        var response = await httpClient.CallCreateTodo(new
        {
            title = "Learn to code",
        });

        await response.Should().HaveStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturn201WhenAllOk()
    {
        var httpClient = await CreateHttpClientAuthenticatedAsNewUser();

        var response = await httpClient.CallCreateTodo(new CreateTodoDto
        {
            Title = "Learn Ruby"
        });

        await response.Should().HaveStatusCode(201);
    }

    [Fact]
    public async Task ShouldAssignNewStatusWhenNoneProvided()
    {
        var httpClient = await CreateHttpClientAuthenticatedAsNewUser();

        var createdTodoId = await httpClient.DoCreateTodo(new CreateTodoDto
        {
            Title = "Learn Ruby"
        });

        var createdTodo = await httpClient.DoGetTodoById(createdTodoId);

        createdTodo!.StatusId.Should().Be((int)TodoStatus.New);
    }
}
