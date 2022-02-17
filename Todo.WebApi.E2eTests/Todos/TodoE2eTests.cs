using FluentAssertions;
using System.Threading.Tasks;
using Todo.Domain.Enums;
using Todo.WebApi.E2eTests.Models;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
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

        var todoId = await httpClient.DoCreateTodo(new CreateTodoDto
        {
            Title = "Clean bike",
            StatusId = (int)TodoStatus.InProgress,
        });

        var getByIdResponse = await httpClient.CallGetTodoById(todoId);

        var expected = new TodoDetailsDto
        {
            Id = todoId,
            Title = "Clean bike",
            StatusId = (int)TodoStatus.InProgress,
            StatusName = "In progress",
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
