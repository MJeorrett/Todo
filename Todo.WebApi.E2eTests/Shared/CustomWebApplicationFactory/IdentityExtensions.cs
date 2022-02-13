using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Todo.Infrastructure.Identity;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Dtos.Identity;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Todo.WebApi.E2eTests.Shared.Extensions;
using Todo.WebApi.E2eTests.Shared.Models;
using Xunit;

namespace Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;

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

    public static async Task<string> CreateAspNetUser(
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

        var createResult = await userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            throw new Exception("Failed to create application user.");
        }

        return user.Id;
    }

    public static async Task<HttpClient> CreateHttpClientAuthenticatedAsUser(
        this CustomWebApplicationFactory factory,
        ClientApplicationDetails clientApplication,
        string userName,
        string password)
    {
        var clientId = clientApplication.ClientId;
        var scope = clientApplication.Scope;
        var redirectUri = clientApplication.RedirectUri;

        var codeChallenge = "F0j7nFUUJXTZyuxEHqzaRzFUfuyPymA2Rt-LsJqO_YQ";
        const string codeVerifier = "ji3CfZGexmk8swkMdwyI2oq5iZ5FLeNWNZKUscOi6NDzLTiQUeOL90nOf_mE2-_Wqj8zkRKKDOuNxkEHcuy7MQ";

        var httpClient = factory.CreateClient();
        var getLoginPageResponse = await httpClient.CallGetLoginPage(clientId, redirectUri, codeChallenge);

        await getLoginPageResponse.Should().HaveStatusCode(200);
        string antiforgeryToken = await ParseAntiforgeryTokenFromLoginPage(getLoginPageResponse);

        var postLoginResponse = await httpClient.CallPostLogin(
            clientId, scope, redirectUri, codeChallenge, userName, password, antiforgeryToken);

        await postLoginResponse.Should().HaveStatusCode(200);

        var postLoginResponseQuery = HttpUtility.ParseQueryString(postLoginResponse.RequestMessage!.RequestUri!.Query);
        var code = postLoginResponseQuery[0];

        code.Should().NotBeEmpty();

        var getTokenResponse = await httpClient.PostToken(
            clientId, redirectUri, code!, codeVerifier);

        await getTokenResponse.Should().HaveStatusCode(200);

        var getTokenResponseBody = await getTokenResponse.Content.ReadFromJsonAsync<GetTokenResponse>();

        getTokenResponseBody.Should().NotBeNull();
        getTokenResponseBody!.access_token.Should().NotBeEmpty();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getTokenResponseBody?.access_token);

        return httpClient;
    }

    private static async Task<string> ParseAntiforgeryTokenFromLoginPage(HttpResponseMessage getLoginPageResponse)
    {
        var loginPageHtml = await getLoginPageResponse.Content.ReadAsStringAsync();
        var loginPageHtmlDocument = new HtmlDocument();
        loginPageHtmlDocument.LoadHtml(loginPageHtml);
        var antiforgeryTokenInput = loginPageHtmlDocument.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']");
        var antiforgeryToken = antiforgeryTokenInput.GetAttributeValue("value", "");
        return antiforgeryToken;
    }
}
