using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Commands;

[Collection("waf")]
public class CreateTodoValidationTests : TestBase, IAsyncLifetime
{
    private HttpClient _httpClient = null!;

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
    public async Task ShouldReturn400WhenNoTitleProvied()
    {
        var actualResult = await _httpClient.CallCreateTodo(new { });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsEmptyString()
    {
        var actualResult = await _httpClient.CallCreateTodo(new
        {
            title = "",
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn400WhenTitleIsToLong()
    {
        var actualResult = await _httpClient.CallCreateTodo(new
        {
            title = new string('a', 257),
        });

        await actualResult.Should().HaveStatusCode400WithErrorForField("Title");
    }

    [Fact]
    public async Task ShouldReturn200WhenAllOk()
    {
        var actualResult = await _httpClient.CallCreateTodo(new
        {
            title = new string('a', 256),
        });

        await actualResult.Should().HaveStatusCode(201);
    }
}
