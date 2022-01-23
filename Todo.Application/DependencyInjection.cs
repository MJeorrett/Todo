using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Todo.Application.Todos.Commands.Create;

namespace Todo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }

    public static IMvcBuilder AddApplicationFluentValidation(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblyContaining<CreateTodoCommandValidator>();
            fv.DisableDataAnnotationsValidation = true;
            fv.ImplicitlyValidateChildProperties = true;
        });

        return mvcBuilder;
    }
}
