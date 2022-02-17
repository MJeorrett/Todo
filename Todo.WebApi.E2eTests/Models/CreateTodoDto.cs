namespace Todo.WebApi.E2eTests.Models;

internal record CreateTodoDto
{
    public string? Title { get; init; }

    public int? StatusId { get; init; }
}
