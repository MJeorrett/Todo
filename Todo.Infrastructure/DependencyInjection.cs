using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Common.Interfaces;
using Todo.Infrastructure.DateTimes;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDateTimes();
        services.AddPersistence(configuration);

        return services;
    }

    private static void AddDateTimes(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SqlServer");

        if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Missing configuration for SqlServer ConnectionString.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }
}
