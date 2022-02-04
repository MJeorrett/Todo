using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Threading.Tasks;
using Todo.Infrastructure.Identity;

namespace Todo.WebApi.E2eTests.WebApplicationFactory;

public static class IdentityExtensions
{
    public static async Task CreatePkceClientApplication(
        this CustomWebApplicationFactory factory,
        string clientId,
        string scope = "api",
        string redirectUri = "http://localhost:3333")
    {
        var serviceScope = factory.Services.CreateScope();

        var openIddictManager = serviceScope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await openIddictManager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            DisplayName = clientId + " client",
            RedirectUris = { new Uri(redirectUri) },
            PostLogoutRedirectUris = { new Uri(redirectUri) },
            Permissions =
            {
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.Token,
                OpenIddictConstants.Permissions.Endpoints.Logout,

                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                OpenIddictConstants.Permissions.Prefixes.Scope + scope,

                OpenIddictConstants.Permissions.ResponseTypes.Code
            }
        });
    }

    public static async Task CreateAspNetUser(
        this CustomWebApplicationFactory factory,
        string email,
        string password,
        bool emailConfirmed = true)
    {
        var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser(email)
        {
            Email = email,
            EmailConfirmed = emailConfirmed,
        };

        await userManager.CreateAsync(user, password);
    }
}
