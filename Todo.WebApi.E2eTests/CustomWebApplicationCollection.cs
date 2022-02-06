using Xunit;

namespace Todo.WebApi.E2eTests;

[CollectionDefinition("waf")]
public class CustomWebApplicationCollection : ICollectionFixture<WebApplicationFixture>
{
}
