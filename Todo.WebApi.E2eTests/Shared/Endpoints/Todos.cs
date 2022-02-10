using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Todo.WebApi.E2eTests.Shared.Endpoints;

public static class Todos
{
    public static async Task<HttpResponseMessage> CallCreateTodo(this HttpClient httpClient, object requestBody)
    {
        return await httpClient.PostAsJsonAsync("api/todos", requestBody);
    }

    public static async Task<HttpResponseMessage> CallGetTodoById(this HttpClient httpClient, int todoId)
    {
        return await httpClient.GetAsync($"api/todos/{todoId}");
    }

    public static async Task<HttpResponseMessage> CallListTodos(this HttpClient httpClient, int pageNumber, int pageSize)
    {
        return await httpClient.GetAsync($"api/todos?pageNumber={pageNumber}&pageSize={pageSize}");
    }

    public static async Task<HttpResponseMessage> CallUpdateTodo(this HttpClient httpClient, object requestBody)
    {
        return await httpClient.PutAsJsonAsync($"api/todos", requestBody);
    }
}
