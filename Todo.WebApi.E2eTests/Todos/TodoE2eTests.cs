using FluentAssertions;
using System.Threading.Tasks;
using Todo.Application.Todos;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos;

[Collection("waf")]
public class TodoE2eTests : TestBase
{
    public TodoE2eTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    [Fact]
    public async Task ShouldPopulateAllProperties()
    {
        var userId = await Factory.CreateAspNetUser("test@mailinator.com", "Sitekit123!");
        var httpClient = await CreateHttpClientAuthenticatedAsUser("test@mailinator.com", "Sitekit123!");

        var todoId = await CreateTodo(httpClient, new
        {
            title = "Clean bike",
        });

        var getByIdResponse = await httpClient.CallGetTodoById(todoId);

        var expected = new TodoDetailsDto
        {
            Id = todoId,
            Title = "Clean bike",
            CreatedAt = MockedNowDateTimeUtc,
            CreatedBy = userId,
            LastUpdatedAt = null,
            LastUpdatedBy = "",
        };

        await getByIdResponse.Should().HaveStatusCode(200);
        (await getByIdResponse.Should().ContainAppResponseOfType<TodoDetailsDto>())
            .Which.Content.Should().BeEquivalentTo(expected);
    }
}
