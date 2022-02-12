﻿using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
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

        await response.Should().BeStatusCode(401);
    }

    [Fact]
    public async Task ShouldReturn201WhenAllOk()
    {
        var httpClient = await CreateUserAndAuthenticatedHttpClient("test@mailinator.com", "Sitekit123!");

        var response = await httpClient.CallCreateTodo(new
        {
            title = "Learn to code",
        });

        await response.Should().BeStatusCode(201);
    }
}
