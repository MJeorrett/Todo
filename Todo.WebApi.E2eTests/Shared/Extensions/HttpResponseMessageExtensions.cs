using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T?> ReadResponseContentAs<T>(this HttpResponseMessage message)
    {
        var response = await message.Content.ReadFromJsonAsync<AppResponse<T>>();

        return response is null ? default : response.Content;
    }

    public static async Task<(bool, T?)> TryReadResponseContentAs<T>(this HttpResponseMessage message)
    {
        try
        {
            var content = await message.ReadResponseContentAs<T>();

            return (true, content);
        }
        catch (Exception)
        {
            return (false, default);
        }
    }
}
