using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using Todo.Infrastructure.Identity;

namespace Todo.Infrastructure.Persistence.Seeding;

public class AuthDataSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public AuthDataSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        await EnsureDefaultUserCreated(scope);
        await EnsureDefaultClientApplicationSeeded(scope, cancellationToken);
    }

    private static async Task EnsureDefaultUserCreated(IServiceScope scope)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string email = "default-dev@mailinator.com";
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

    private static async Task EnsureDefaultClientApplicationSeeded(IServiceScope scope, CancellationToken cancellationToken)
    {
        var openIddictManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        const string clientId = "default-dev";
        const string redirectUri = "https://oauth.pstmn.io/v1/callback";

        var existingClient = await openIddictManager.FindByClientIdAsync(clientId, cancellationToken);

        if (existingClient is null)
        {
            await openIddictManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = "default-dev-secret",
                DisplayName = "Default Dev",
                RedirectUris = { new Uri(redirectUri) },
                PostLogoutRedirectUris = { new Uri(redirectUri) },
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
