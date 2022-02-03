using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;

namespace Todo.Application.Todos.Queries.GetById;

public record GetTodoByIdQuery
{
    public int TodoId { get; init; }
}

public class GetTodoByIdQueryHandler : IRequestHandler<GetTodoByIdQuery, TodoDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTodoByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppResponse<TodoDetailsDto>> Handle(
        GetTodoByIdQuery query,
        CancellationToken cancellationToken)
    {
        var todoEntity = await _dbContext.Todos
            .Where(_ => _.Id == query.TodoId)
            .FirstOrDefaultAsync(cancellationToken);

        if (todoEntity == null) return new(404);

        return new(200, new()
        {
            Id = todoEntity.Id,
            Title = todoEntity.Title,
        });
    }
}
