using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<(bool, T?)> TryReadContentAs<T>(this HttpResponseMessage message)
    {
        try
        {
            var content = await message.Content.ReadFromJsonAsync<T>();

            return (true, content);
        }
        catch (Exception)
        {
            return (false, default);
        }
    }
}
