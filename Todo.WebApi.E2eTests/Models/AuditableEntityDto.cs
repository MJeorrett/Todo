using System;

namespace Todo.WebApi.E2eTests.Models;

internal abstract record AuditableEntityDto
{
    public int Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string CreatedBy { get; init; } = "";

    public DateTime? LastUpdatedAt { get; init; }

    public string LastUpdatedBy { get; init; } = "";
}
