using System;

namespace Todo.WebApi.E2eTests.Dtos;

internal record TodoDetailsDto(
    int Id,
    string Title,
    DateTime CreatedAt,
    DateTime? LastUpdatedAt)
{

}
