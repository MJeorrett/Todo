using Xunit;

namespace Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;

[CollectionDefinition("waf")]
public class CustomWebApplicationCollection : ICollectionFixture<WebApplicationFixture>
{
}
