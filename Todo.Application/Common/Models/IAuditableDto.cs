using NodaTime;

namespace Todo.Application.Common.Models;

public interface IAuditableDto
{
    DateTime CreatedAt { get; }

    string CreatedBy { get; }

    DateTime? LastUpdatedAt { get; }

    string LastUpdatedBy { get; }
}
