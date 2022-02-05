using NodaTime;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Domain.Common;

public abstract class AuditableEntity
{
    public ZonedDateTime CreatedAt { get; set; }

    [Column(TypeName = "nvarchar(36)")]
    public string CreatedBy { get; set; } = "";

    public ZonedDateTime? LastUpdatedAt { get; set; }

    [Column(TypeName = "varchar(36)")]
    public string LastUpdatedBy { get; set; } = "";
}
