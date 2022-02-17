using Todo.Application.Common.Models;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.Extensions;

namespace Todo.Application.Todos;

public record TodoDetailsDto : AuditableEntityDto
{
    public string Title { get; init; } = "";

    public TodoStatus StatusId { get; init; }

    public string StatusName { get; init; } = "";

    public static TodoDetailsDto FromEntity(TodoEntity todoEntity)
    {
        return new()
        {
            Id = todoEntity.Id,
            Title = todoEntity.Title,
            StatusId = todoEntity.Status,
            StatusName = todoEntity.Status.GetUserFriendlyName(),
            CreatedAt = todoEntity.CreatedAt.ToDateTimeUtc(),
            CreatedBy = todoEntity.CreatedBy,
            LastUpdatedAt = todoEntity.LastUpdatedAt?.ToDateTimeUtc(),
            LastUpdatedBy = todoEntity.LastUpdatedBy,
        };
    }
}
