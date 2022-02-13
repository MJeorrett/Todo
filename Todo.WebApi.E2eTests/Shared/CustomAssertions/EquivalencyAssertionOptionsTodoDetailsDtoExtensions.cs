using FluentAssertions.Equivalency;
using Todo.Application.Todos;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

public static class EquivalencyAssertionOptionsTodoDetailsDtoExtensions
{
    public static EquivalencyAssertionOptions<TodoDetailsDto> ExcludingCreatedAtAndBy(this EquivalencyAssertionOptions<TodoDetailsDto> target)
    {
        return target.Excluding(memberInfo =>
            memberInfo.Name == nameof(TodoDetailsDto.CreatedAt) ||
            memberInfo.Name == nameof(TodoDetailsDto.CreatedBy));
    }
}
