using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task AssertIsStatusCode(this HttpResponseMessage message, int expectedStatusCode)
    {
        if (message.StatusCode == (HttpStatusCode)expectedStatusCode) return;

        var responseContent = await message.Content.ReadAsStringAsync();
        var errorMessage = $"Expected status code {expectedStatusCode} but recieved status code {(int)message.StatusCode} with " +
            (string.IsNullOrEmpty(responseContent) ?
                "no content." :
                $"content:\n{responseContent}");

        Assert.True(false, errorMessage);
    }

    public static async Task AssertIs400WithErrorForField(
           this HttpResponseMessage message,
           string expectedErrorField)
    {
        await message.AssertIsStatusCode(400);

        var responseContent = await message.Content.ReadAsStringAsync();
        var responseJson = JsonDocument.Parse(responseContent);

        if (responseJson.RootElement.TryGetProperty("errors", out var errors))
        {
            var hasExpectedError = errors.TryGetProperty(expectedErrorField, out var _);
            var errorKeys = string.Join(",", errors.EnumerateObject().Select(_ => _.Name));
            Assert.True(hasExpectedError, $"Expected error for: {expectedErrorField}\nFound error key(s): {errorKeys}");
        }
        else
        {
            Assert.True(false, $"No 'errors' key found on the response content:\n{responseJson}");
        }
    }

    public static async Task<T?> ReadResponseContentAs<T>(this HttpResponseMessage message)
    {
        var response = await message.Content.ReadFromJsonAsync<AppResponse<T>>();

        return response is null ? default : response.Content;
    }
}
