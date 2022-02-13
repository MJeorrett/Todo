using System.Net.Http;
using System.Threading.Tasks;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Xunit;

namespace Todo.WebApi.E2eTests.Todos.Queries;

[Collection("waf")]
public class ListTodosQueryValidationTests : TestBase, IAsyncLifetime
{
    private HttpClient _httpClient = null!;

    public ListTodosQueryValidationTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _httpClient = await CreateHttpClientAuthenticatedAsNewUser();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldReturn400WhenPageNumberIsLessThan0(int invalidPageNumber)
    {
        var actualResult = await _httpClient.CallListTodos(invalidPageNumber, 1);

        await actualResult.Should().BeStatusCode400WithErrorForField("PageNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ShouldReturn400WhenPageSizeIsLessThan0(int invalidPageSize)
    {
        var actualResult = await _httpClient.CallListTodos(1, invalidPageSize);

        await actualResult.Should().BeStatusCode400WithErrorForField("PageSize");
    }
}
