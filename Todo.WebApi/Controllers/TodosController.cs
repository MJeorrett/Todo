using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos;
using Todo.Application.Todos.Commands.Create;
using Todo.Application.Todos.Queries.GetById;
using Todo.WebApi.Extensions;

namespace Todo.WebApi.Controllers;

[ApiController]
public class TodosController : ControllerBase
{
    [HttpPost("api/todos")]
    public async Task<ActionResult<AppResponse<int>>> CreateTodo(
            [FromServices] IMediator mediator,
            [FromBody] CreateTodoCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToActionResult();
    }

    [HttpGet("api/todos/{todoId}")]
    public async Task<ActionResult<AppResponse<TodoDetailsDto>>> GetTodoById(
           [FromServices] IMediator mediator,
           [FromRoute] int todoId)
    {
        var query = new GetTodoByIdQuery() { TodoId = todoId };

        var response = await mediator.Send(query);
        return response.ToActionResult();
    }
}
