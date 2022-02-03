using Todo.Domain.Common;

namespace Todo.Domain.Entities;

public class TodoEntity : AuditableEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
}
