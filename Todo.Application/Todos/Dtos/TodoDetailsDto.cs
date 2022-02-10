using NodaTime;
using Todo.Application.Common.Models;
using Todo.Domain.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Todos;

public record TodoDetailsDto : IAuditableDto, IEntity
{
    public int Id { get; init; }

    public string Title { get; init; } = "";

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = "";

    public DateTime? LastUpdatedAt { get; init; }

    public string LastUpdatedBy { get; init; } = "";

    public static TodoDetailsDto FromEntity(TodoEntity todoEntity)
    {
        return new()
        {
            Id = todoEntity.Id,
            Title = todoEntity.Title,
            CreatedAt = todoEntity.CreatedAt.ToDateTimeUtc(),
            CreatedBy = todoEntity.CreatedBy,
            LastUpdatedAt = todoEntity.LastUpdatedAt?.ToDateTimeUtc(),
            LastUpdatedBy = todoEntity.LastUpdatedBy,
        };
    }
}
