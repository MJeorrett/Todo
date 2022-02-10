namespace Todo.WebApi.E2eTests.Shared.Dtos;

public record PaginatedListQuery
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
