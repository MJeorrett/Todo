using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Todo.WebApi.E2eTests.Shared.Endpoints;

public static class Authorization
{
    public static async Task<HttpResponseMessage> GetLoginPage(
        this HttpClient httpClient,
        string clientId,
        string redirectUri,
        string codeChallenge)
    {
        var uri = $"/connect/authorize?response_type=" +
            $"code&" +
            $"client_id={clientId}&" +
            $"redirect_uri={HttpUtility.UrlEncode(redirectUri)}&" +
            $"code_challenge={codeChallenge}&" +
            $"code_challenge_method=S256";

        return await httpClient.GetAsync(uri);
    }

    public static async Task<HttpResponseMessage> PostLogin(
        this HttpClient httpClient,
        string clientId,
        string scope,
        string redirectUri,
        string codeChallenge,
        string userEmail,
        string userPassword,
        string antiforgeryToken)
    {
        var returnUrl = $"/connect/authorize?" +
            $"client_id={clientId}&" +
            $"scope={scope}&" +
            $"response_type=code&" +
            $"redirect_uri={HttpUtility.UrlEncode(redirectUri)}&" +
            $"code_challenge={codeChallenge}&" +
            $"code_challenge_method=S256";

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("ReturnUrl", returnUrl),
            new KeyValuePair<string, string>("UserName", userEmail),
            new KeyValuePair<string, string>("Password", userPassword),
            new KeyValuePair<string, string>("__RequestVerificationToken", antiforgeryToken),
        });

        return await httpClient.PostAsync("/account/login", content);
    }

    public static async Task<HttpResponseMessage> PostToken(
        this HttpClient httpClient,
        string clientId,
        string redirectUri,
        string code,
        string codeVerifier)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code!),
            new KeyValuePair<string, string>("code_verifier", codeVerifier),
        });

        return await httpClient.PostAsync("/connect/token", content);
    }
}
