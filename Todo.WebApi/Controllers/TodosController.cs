using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos;
using Todo.Application.Todos.Commands.Create;
using Todo.Application.Todos.Commands.Update;
using Todo.Application.Todos.Queries.GetById;
using Todo.WebApi.Extensions;

namespace Todo.WebApi.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class TodosController : ControllerBase
{
    [HttpPost("api/todos")]
    public async Task<ActionResult<AppResponse<int>>> CreateTodo(
            [FromServices] CreateTodoCommandHandler handler,
            [FromBody] CreateTodoCommand command,
            CancellationToken cancellationToken)
    {
        var response = await handler.Handle(command, cancellationToken);
        return response.ToActionResult();
    }

    [HttpGet("api/todos/{todoId}")]
    public async Task<ActionResult<AppResponse<TodoDetailsDto>>> GetTodoById(
           [FromServices] GetTodoByIdQueryHandler handler,
           [FromRoute] int todoId,
           CancellationToken cancellationToken)
    {
        var query = new GetTodoByIdQuery() { TodoId = todoId };

        var response = await handler.Handle(query, cancellationToken);
        return response.ToActionResult();
    }

    [HttpPut("api/todos")]
    public async Task<ActionResult<AppResponse>> UpdateTodo(
           [FromServices] UpdateTodoCommandHandler handler,
           [FromBody] UpdateTodoCommand command,
           CancellationToken cancellationToken)
    {
        var response = await handler.Handle(command, cancellationToken);
        return response.ToActionResult();
    }
}
