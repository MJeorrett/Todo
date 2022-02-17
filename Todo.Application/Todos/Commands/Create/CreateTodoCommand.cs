using Microsoft.Extensions.Logging;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Todos.Commands.Create;

public record CreateTodoCommand
{
    public string Title { get; init; } = null!;

    public TodoStatus? StatusId { get; init; }
}

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, int>
{
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public CreateTodoCommandHandler(
        ILogger<CreateTodoCommandHandler> logger,
        IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AppResponse<int>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating todo {todoTitle}.", command.Title);

        var todoEntity = new TodoEntity()
        {
            Title = command.Title,
            Status = command.StatusId ?? TodoStatus.New,
        };

        _dbContext.Todos.Add(todoEntity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created todo with id {todoId}.", todoEntity.Id);

        return new(201, todoEntity.Id);
    }
}
