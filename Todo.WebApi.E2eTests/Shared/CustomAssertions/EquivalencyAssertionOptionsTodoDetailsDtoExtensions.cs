using FluentAssertions.Equivalency;
using Todo.WebApi.E2eTests.Models;

namespace Todo.WebApi.E2eTests.Shared.Extensions;

internal static class EquivalencyAssertionOptionsTodoDetailsDtoExtensions
{
    public static EquivalencyAssertionOptions<TodoDetailsDto> ExcludingCreatedAtAndBy(this EquivalencyAssertionOptions<TodoDetailsDto> target)
    {
        return target.Excluding(memberInfo =>
            memberInfo.Name == nameof(TodoDetailsDto.CreatedAt) ||
            memberInfo.Name == nameof(TodoDetailsDto.CreatedBy));
    }
}
