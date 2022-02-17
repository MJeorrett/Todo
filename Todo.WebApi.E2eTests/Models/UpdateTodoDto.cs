namespace Todo.WebApi.E2eTests.Models;

internal record UpdateTodoDto
{
    public int? TodoId { get; init; }

    public string? Title { get; init; }

    public int? StatusId { get; init; }
}
