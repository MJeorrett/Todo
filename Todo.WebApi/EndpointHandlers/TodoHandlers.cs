using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Todos.Commands.Create;

namespace Todo.WebApi.EndpointHandlers
{
    public static class TodoHandlers
    {
        public static void AddTodoHandlers(this WebApplication app)
        {
            app.MapPost("/api/todos", async (
                [FromServices] IMediator mediator,
                [FromBody] CreateTodoCommand command) =>
                {
                    return await mediator.Send(command);
                })
                .WithName("CreateTodo");
        }
    }
}
