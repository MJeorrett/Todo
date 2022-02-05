using NUnit.Framework;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Dtos.Todos;
using Todo.WebApi.E2eTests.Endpoints;
using Todo.WebApi.E2eTests.WebApplicationFactory;

namespace Todo.WebApi.E2eTests.Todos;

public class CreateTodoTests : TestBase
{
    [Test]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var httpClient = Factory.CreateClient();

        var response = await httpClient.CreateTodo(new
        {
            title = "not me",
        });

        await response.AssertIsStatusCode(401);
    }
}
