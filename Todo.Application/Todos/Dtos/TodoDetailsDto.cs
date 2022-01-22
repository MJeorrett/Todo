namespace Todo.Application.Todos;

public record TodoDetailsDto
{
    public int Id { get; init; }

    public string Title { get; init; }
}
