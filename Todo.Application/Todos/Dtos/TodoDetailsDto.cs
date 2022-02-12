using NodaTime;
using Todo.Application.Common.Models;
using Todo.Domain.Common;
using Todo.Domain.Entities;

namespace Todo.Application.Todos;

public record TodoDetailsDto : AuditableEntityDto
{
    public string Title { get; init; } = "";

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
