using FluentAssertions;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Todo.WebApi.E2eTests.Shared.CustomWebApplicationFactory;
using Todo.WebApi.E2eTests.Shared.Dtos.Identity;
using Todo.WebApi.E2eTests.Shared.Endpoints;
using Xunit;

namespace Todo.WebApi.E2eTests.Identity;

[Collection("waf")]
public class AuthenticationTests : TestBase
{
    public AuthenticationTests(WebApplicationFixture webApplicationFixture) :
        base(webApplicationFixture.Factory)
    {
        
    }

    [Fact]
    public async Task ShouldBeAbleToLogInWithPkceFlow()
    {
        var httpClient = Factory.CreateClient();

        const string clientId = "e2e-test-client";
        const string redirectUri = "http://localhost:3123";
        const string scope = "api";

        var codeChallenge = "F0j7nFUUJXTZyuxEHqzaRzFUfuyPymA2Rt-LsJqO_YQ";
        const string codeVerifier = "ji3CfZGexmk8swkMdwyI2oq5iZ5FLeNWNZKUscOi6NDzLTiQUeOL90nOf_mE2-_Wqj8zkRKKDOuNxkEHcuy7MQ";

        await Factory.CreatePkceClientApplication(clientId, scope, redirectUri);

        const string userEmail = "test-user@mailinator.com";
        const string userPassword = "Sitekit123!";

        await Factory.CreateAspNetUser(userEmail, userPassword);

        var getLoginPageResponse = await httpClient.CallGetLoginPage(clientId, redirectUri, codeChallenge);

        getLoginPageResponse.EnsureSuccessStatusCode();
        string antiforgeryToken = await ParseAntiforgeryTokenFromLoginPage(getLoginPageResponse);

        var postLoginResponse = await httpClient.CallPostLogin(
            clientId, scope, redirectUri, codeChallenge, userEmail, userPassword, antiforgeryToken);

        postLoginResponse.EnsureSuccessStatusCode();

        var postLoginResponseQuery = HttpUtility.ParseQueryString(postLoginResponse.RequestMessage!.RequestUri!.Query);
        var code = postLoginResponseQuery[0];

        code.Should().NotBeEmpty();

        var getTokenResponse = await httpClient.PostToken(
            clientId, redirectUri, code!, codeVerifier);
        
        getTokenResponse.EnsureSuccessStatusCode();

        var getTokenResponseBody = await getTokenResponse.Content.ReadFromJsonAsync<GetTokenResponse>();

        getTokenResponseBody.Should().NotBeNull();
        getTokenResponseBody!.access_token.Should().NotBeEmpty();
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
