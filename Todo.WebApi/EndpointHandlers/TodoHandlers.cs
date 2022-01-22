using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos;
using Todo.Application.Todos.Commands.Create;
using Todo.Application.Todos.Queries.GetById;
using Todo.WebApi.Common;

namespace Todo.WebApi.EndpointHandlers;

public static class TodoHandlers
{
    public static void AddTodoHandlers(this WebApplication app)
    {
        app.MapPost("/api/todos", async (
            [FromServices] IMediator mediator,
            [FromBody] CreateTodoCommand command) =>
        {
            return ResultBuilder.Build(
                await mediator.Send(command));
        })
            .Produces<AppResponse<int>>(201)
            .WithName("CreateTodo");

        app.MapGet("/api/todos/{todoId}", async (
            [FromServices] IMediator mediator,
            [FromRoute] int todoId) =>
        {
            var query = new GetTodoByIdQuery() { TodoId = todoId };

            return ResultBuilder.Build(
                await mediator.Send(query));
        })
            .Produces<AppResponse<TodoDetailsDto>>(200)
            .WithName("GetTodoById");
    }
}
