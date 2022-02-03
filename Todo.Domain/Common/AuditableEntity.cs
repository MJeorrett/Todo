using NodaTime;

namespace Todo.Domain.Common;

public abstract class AuditableEntity
{
    public ZonedDateTime CreatedAt { get; set; }

    public ZonedDateTime? LastUpdatedAt { get; set; }
}
