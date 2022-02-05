using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Common.AppRequests;
using Todo.Application.Todos.Commands.Create;

namespace Todo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.Scan(scan =>
        {
            scan.FromAssemblyOf<CreateTodoCommandHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                .AsSelf()
                .WithScopedLifetime();

            scan.FromAssemblyOf<CreateTodoCommandHandler>()
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
                .AsSelf()
                .WithScopedLifetime();
        });

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
