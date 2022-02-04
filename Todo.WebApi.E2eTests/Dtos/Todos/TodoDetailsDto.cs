using System;

namespace Todo.WebApi.E2eTests.Dtos.Todos;

internal record TodoDetailsDto(
    int Id,
    string Title,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt)
{

}
