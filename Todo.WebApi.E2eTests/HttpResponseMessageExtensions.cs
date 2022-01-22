using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Xunit;

namespace Todo.WebApi.E2eTests;

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

    public static async Task<T?> ReadResponseContentAs<T>(this HttpResponseMessage message)
    {
        var response = await message.Content.ReadFromJsonAsync<AppResponse<T>>();

        return response is null ? default : response.Content;
    }
}
