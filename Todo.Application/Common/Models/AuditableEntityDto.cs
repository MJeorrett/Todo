using Todo.Domain.Common;

namespace Todo.Application.Common.Models;

public abstract record AuditableEntityDto : IEntity
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = "";

    public DateTime? LastUpdatedAt { get; init; }

    public string LastUpdatedBy { get; init; } = "";
}
