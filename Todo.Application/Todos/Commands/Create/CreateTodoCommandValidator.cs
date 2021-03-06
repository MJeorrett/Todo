using FluentValidation;

namespace Todo.Application.Todos.Commands.Create;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(_ => _.Title).TodoTitleRules();

        RuleFor(_ => _.StatusId).IsInEnum();
    }
}
