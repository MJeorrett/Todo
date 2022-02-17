using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Enums;

namespace Todo.Application.Todos.Commands.Update;

public record UpdateTodoCommand
{
    public int TodoId { get; init; }

    public string Title { get; init; } = "";

    public TodoStatus StatusId { get; init; }
}

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand>
{
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public UpdateTodoCommandHandler(
        ILogger<UpdateTodoCommandHandler> logger,
        IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AppResponse> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating todo {todoId}.", command.TodoId);

        var existingTodo = await _dbContext.Todos
            .FirstOrDefaultAsync(_ => _.Id == command.TodoId);

        if (existingTodo is null) return new AppResponse(404);

        existingTodo.Title = command.Title;
        existingTodo.Status = command.StatusId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully updated todo {todoId}.", command.TodoId);
        return new(200);
    }
}
