using System;

namespace Todo.WebApi.E2eTests.Shared.Dtos.Todos;

internal record TodoDetailsDto(
    int Id,
    string Title,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt)
{

}
