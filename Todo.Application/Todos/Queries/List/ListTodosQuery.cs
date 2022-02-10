using Microsoft.Extensions.Logging;
using Todo.Application.Common.AppRequests;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Application.Todos.Queries.List;

public record ListTodosQuery : PaginatedListQuery
{
}

public class ListTodosQueryHandler : IRequestHandler<ListTodosQuery, PaginatedListResponse<TodoDetailsDto>>
{
    private readonly ILogger _logger;
    private readonly IApplicationDbContext _dbContext;

    public ListTodosQueryHandler(
        ILogger<ListTodosQueryHandler> logger,
        IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AppResponse<PaginatedListResponse<TodoDetailsDto>>> Handle(
        ListTodosQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing todos.");

        var todosQueryable = _dbContext.Todos.OrderBy(_ => _.Title);

        var result = await PaginatedListResponse<TodoDetailsDto>.CreateAsync(
            todosQueryable,
            query,
            TodoDetailsDto.FromEntity);

        _logger.LogInformation("Successfully listed todos.");

        return new(200, result);
    }
}
