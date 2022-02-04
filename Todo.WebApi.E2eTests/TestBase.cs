using NUnit.Framework;
using Todo.WebApi.E2eTests.WebApplicationFactory;

namespace Todo.WebApi.E2eTests;

public class TestBase
{
    protected CustomWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public void Initialize()
    {
        _factory = new CustomWebApplicationFactory();
    }
}
