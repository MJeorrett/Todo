using HtmlAgilityPack;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using Todo.WebApi.E2eTests.Dtos.Identity;
using Todo.WebApi.E2eTests.WebApplicationFactory;

namespace Todo.WebApi.E2eTests.Identity;

public class AuthenticationTests : TestBase
{
    [Test]
    public async Task ShouldBeAbleToLogInWithPkceFlow()
    {
        var httpClient = _factory.CreateClient();

        const string clientId = "e2e-test-client";
        const string redirectUri = "http://localhost:3123";
        const string scope = "api";

        var codeChallenge = "F0j7nFUUJXTZyuxEHqzaRzFUfuyPymA2Rt-LsJqO_YQ";
        const string codeVerifier = "ji3CfZGexmk8swkMdwyI2oq5iZ5FLeNWNZKUscOi6NDzLTiQUeOL90nOf_mE2-_Wqj8zkRKKDOuNxkEHcuy7MQ";

        await _factory.CreatePkceClientApplication(clientId, scope, redirectUri);
        
        const string userEmail = "test-user@mailinator.com";
        const string userPassword = "Sitekit123!";

        await _factory.CreateAspNetUser(userEmail, userPassword);

        var uri = $"/connect/authorize?response_type=code&client_id={clientId}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256";
        var getLoginPageResponse = await httpClient.GetAsync(uri);
        getLoginPageResponse.EnsureSuccessStatusCode();

        var loginPageHtml = await getLoginPageResponse.Content.ReadAsStringAsync();
        var loginPageHtmlDocument = new HtmlDocument();
        loginPageHtmlDocument.LoadHtml(loginPageHtml);
        var antiforgeryTokenInput = loginPageHtmlDocument.DocumentNode.SelectSingleNode("//input[@name='__RequestVerificationToken']");
        var antiforgeryToken = antiforgeryTokenInput.GetAttributeValue("value", "");

        var postLoginContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("ReturnUrl", $"/connect/authorize?client_id={clientId}&scope={scope}%20openid%20offline_access&response_type=code&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&code_challenge={codeChallenge}&code_challenge_method=S256"),
            new KeyValuePair<string, string>("UserName", userEmail),
            new KeyValuePair<string, string>("Password", userPassword),
            new KeyValuePair<string, string>("__RequestVerificationToken", antiforgeryToken),
        });

        var postLoginResponse = await httpClient.PostAsync("/account/login", postLoginContent);
        postLoginResponse.EnsureSuccessStatusCode();

        var postLoginResponseQuery = HttpUtility.ParseQueryString(postLoginResponse.RequestMessage!.RequestUri!.Query);
        var code = postLoginResponseQuery[0];

        var getTokenContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code!),
            new KeyValuePair<string, string>("code_verifier", codeVerifier),
        });

        var getTokenResponse = await httpClient.PostAsync("/connect/token", getTokenContent);
        getTokenResponse.EnsureSuccessStatusCode();

        var getTokenResponseBody = await getTokenResponse.Content.ReadFromJsonAsync<GetTokenResponse>();

        Assert.IsNotEmpty(getTokenResponseBody?.access_token);
    }
}
