using FluentValidation;

namespace Todo.Application.Common.AppRequests;

public class PaginatedListQueryValidator<T> : AbstractValidator<T> where T : PaginatedListQuery
{
    public PaginatedListQueryValidator()
    {
        RuleFor(_ => _.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(_ => _.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100);
    }
}
