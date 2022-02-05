using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Todo.WebApi.E2eTests.Shared.Endpoints;

public static class Todos
{
    public static async Task<HttpResponseMessage> CreateTodo(this HttpClient httpClient, object requestBody)
    {
        return await httpClient.PostAsJsonAsync("api/todos", requestBody);
    }

    public static async Task<HttpResponseMessage> GetTodoById(this HttpClient httpClient, int todoId)
    {
        return await httpClient.GetAsync($"api/todos/{todoId}");
    }
}
