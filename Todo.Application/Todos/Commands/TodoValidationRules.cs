using FluentValidation;

namespace Todo.Application.Todos.Commands;

public static class TodoValidationRules
{
    public static IRuleBuilder<T, string> TodoTitleRules<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(256);
    }
}
