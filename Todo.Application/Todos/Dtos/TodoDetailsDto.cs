namespace Todo.Application.Todos;

public record TodoDetailsDto
{
    public int Id { get; init; }

    public string Title { get; init; } = "";

    public DateTime CreatedAt { get; init; }

    public DateTime? LastUpdatedAt { get; init; }
}
