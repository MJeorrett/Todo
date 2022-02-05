using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NodaTime;
using Respawn;
using Respawn.Graph;
using System.Linq;
using System.Threading.Tasks;
using Todo.Application.Common.Interfaces;

namespace Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public ZonedDateTime MockedNow { get; set; } = SystemClock.Instance.GetCurrentInstant().InUtc();

    private Checkpoint _checkpoint = null!;

    public async Task ResetState()
    {
        var configuration = GetRequiredScopedService<IConfiguration>();

        await _checkpoint.Reset(configuration.GetConnectionString("SqlServer"));
    }

    public T GetRequiredScopedService<T>() where T : notnull
    {
        return Services.CreateScope().ServiceProvider.GetRequiredService<T>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            MockDateTimeService(services);
        });

        _checkpoint = new Checkpoint
        {
            TablesToIgnore = new[] { new Table("__EFMigrationsHistory") },
        };

        base.ConfigureWebHost(builder);
    }

    private void MockDateTimeService(IServiceCollection services)
    {
        var serviceDescriptor = services
            .First(_ => _.ServiceType == typeof(IDateTimeService));

        services.Remove(serviceDescriptor);

        services.AddScoped(provider =>
            Mock.Of<IDateTimeService>(_ => _.Now == MockedNow));
    }
}
