using Todo.Domain.Common;
using Todo.Domain.Entities;

namespace Todo.Domain.Enums;

public enum TodoStatus
{
    New = 0,
    InProgress = 1,
    Complete = 2,
    Abandoned = 3,
}

public class TodoStatusEntity : IEnumEntity<TodoStatus>
{
    public TodoStatus Id { get; set; }

    public string Name { get; set; } = "";

    public List<TodoEntity> Todos { get; set; } = new();
}
