namespace Todo.Application.Common.AppRequests;

public record PaginatedListQuery
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
