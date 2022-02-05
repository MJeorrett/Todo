using NUnit.Framework;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

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
