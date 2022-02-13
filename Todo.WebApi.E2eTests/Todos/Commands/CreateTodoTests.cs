using FluentAssertions;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
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

        var response = await httpClient.CallCreateTodo(new
        {
            title = "Learn to code",
        });

        await response.Should().HaveStatusCode(201);
    }
}
