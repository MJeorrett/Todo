using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using Todo.Infrastructure.Identity;
using Todo.Infrastructure.Persistence;

namespace Todo.WebApi;

public class TestDataSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public TestDataSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await EnsureApplicationSeeded(scope, cancellationToken);
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string email = "mjeorrett@mailinator.com";
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is null)
        {
            var user = new ApplicationUser(email)
            {
                Email = email,
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(user, "Sitekit123!");
        }
    }

    private static async Task EnsureApplicationSeeded(IServiceScope scope, CancellationToken cancellationToken)
    {
        var openIddictManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var existingClient = await openIddictManager.FindByClientIdAsync("postman", cancellationToken);

        if (existingClient is null)
        {
            await openIddictManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ClientSecret = "postman-secret",
                DisplayName = "Postman",
                RedirectUris = { new Uri("http://localhost:3000") },
                PostLogoutRedirectUris = { new Uri("http://localhost:3000") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Logout,

                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                    OpenIddictConstants.Permissions.Prefixes.Scope + "api",

                    OpenIddictConstants.Permissions.ResponseTypes.Code
                }
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
