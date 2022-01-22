using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;

namespace Todo.Application.Todos.Queries.GetById;

public record GetTodoByIdQuery : IAppRequest<TodoDetailsDto>
{
    public int TodoId { get; init; }
}

public class GetByIdQueryHandler : IAppRequestHandler<GetTodoByIdQuery, TodoDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AppResponse<TodoDetailsDto>> Handle(
        GetTodoByIdQuery query,
        CancellationToken cancellationToken)
    {
        var todoEntity = await _dbContext.Todos
            .Where(_ => _.Id == query.TodoId)
            .FirstOrDefaultAsync();

        if (todoEntity == null) return new(404);

        return new(200, new()
        {
            Id = todoEntity.Id,
            Title = todoEntity.Title,
        });
    }
}
