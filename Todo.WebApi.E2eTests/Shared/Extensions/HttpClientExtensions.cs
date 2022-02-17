using FluentAssertions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Todo.Application.Common.AppRequests;
using Todo.WebApi.E2eTests.Models;
using Todo.WebApi.E2eTests.Shared.Assertions;
using Todo.WebApi.E2eTests.Shared.Endpoints;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class HttpClientExtensions
{
    internal static async Task<int> DoCreateTodo(this HttpClient httpClient, object body)
    {
        var response = await httpClient.CallCreateTodo(body);

        await response.Should().HaveStatusCode(201);

        var parsedResponse = await response.Content.ReadFromJsonAsync<AppResponse<int>>();

        return parsedResponse!.Content;
    }

    internal static async Task<int> DoCreateTodoWithTitle(this HttpClient httpClient, string title)
    {
        return await httpClient.DoCreateTodo(new { title });
    }

    internal static async Task<List<int>> DoCreateTodosWithTitles(this HttpClient httpClient, params string[] titles)
    {
        var todoIds = new List<int>(titles.Length);

        foreach (var title in titles)
        {
            todoIds.Add(await httpClient.DoCreateTodoWithTitle(title));
        }

        return todoIds;
    }

    internal static async Task<TodoDetailsDto?> DoGetTodoById(this HttpClient httpClient, int todoId)
    {
        var response = await httpClient.CallGetTodoById(todoId);

        await response.Should().HaveStatusCode(200);

        var parsedResponse = await response.Content.ReadFromJsonAsync<AppResponse<TodoDetailsDto>>();

        return parsedResponse!.Content;
    }
}
