using NUnit.Framework;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;

namespace Todo.WebApi.E2eTests.Todos.Commands;

public class CreateTodoTests : TestBase
{
    [Test]
    public async Task ShouldReturn401WhenCallerNotAuthenticated()
    {
        var httpClient = Factory.CreateClient();

        var response = await httpClient.CallCreateTodo(new
        {
            title = "Learn to code",
        });

        await response.AssertIsStatusCode(401);
    }
}
