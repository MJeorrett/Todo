using Todo.Domain.Common;
using Todo.Domain.Enums;

namespace Todo.Domain.Entities;

public class TodoEntity : AuditableEntity, IEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public TodoStatus Status { get; set; }
    public TodoStatusEntity StatusEntity { get; set; } = null!;
}
