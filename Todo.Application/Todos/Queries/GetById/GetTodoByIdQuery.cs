using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;

namespace Todo.Application.Todos.Queries.GetById;

public record GetTodoByIdQuery
{
    public int TodoId { get; init; }
}

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDetailsDto>
{
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public GetTodoByIdQueryHandler(
        ILogger<GetTodoByIdQueryHandler> logger,
        IApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<AppResponse<TodoDetailsDto>> Handle(
        GetTodoByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting todo {todoId} by id.", query.TodoId);

        var todoEntity = await _dbContext.Todos
            .Where(_ => _.Id == query.TodoId)
            .FirstOrDefaultAsync(cancellationToken);

        if (todoEntity == null) return new(404);

        _logger.LogInformation("Successfully retrieved todo {todoId} by id.", query.TodoId);

        return new(200, new()
        {
            Id = todoEntity.Id,
            Title = todoEntity.Title,
            CreatedAt = todoEntity.CreatedAt.ToDateTimeUtc(),
            LastUpdatedAt = todoEntity.LastUpdatedAt?.ToDateTimeUtc(),
        });
    }
}
