using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos.Commands.Create;
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
    }
}
