using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Common.Interfaces;

namespace Todo.Application.Todos.Commands.Update;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(_ => _.Title).TodoTitleRules();

        RuleFor(_ => _.StatusId).IsInEnum();
    }
}
