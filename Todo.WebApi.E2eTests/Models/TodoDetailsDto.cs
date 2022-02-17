namespace Todo.WebApi.E2eTests.Models;

internal record TodoDetailsDto : AuditableEntityDto
{
    public string Title { get; init; } = "";

    public int StatusId { get; init; }

    public string StatusName { get; init; } = "";
}
