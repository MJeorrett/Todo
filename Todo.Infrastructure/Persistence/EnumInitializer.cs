using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todo.Domain.Common;
using Todo.Domain.Extensions;
using Todo.Infrastructure.Exceptions;

namespace Todo.Infrastructure.Persistence;

public class EnumInitializer<TEnum, TEnumEntity> : IHostedService
    where TEnumEntity : class, IEnumEntity<TEnum>, new()
    where TEnum : struct, Enum
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EnumInitializer(
        ILogger<EnumInitializer<TEnum, TEnumEntity>> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existingDbEntities = await dbContext.Set<TEnumEntity>().ToArrayAsync(cancellationToken);

        var enumEntitiesInCode = Enum.GetValues<TEnum>()
            .Select(enumValue => new TEnumEntity()
            {
                Id = enumValue,
                Name = enumValue.GetUserFriendlyName(),
            });

        foreach (var enumEntity in enumEntitiesInCode)
        {
            var existingDbEntity = existingDbEntities.FirstOrDefault(_ => _.Id.Equals(enumEntity.Id));
            AddEnumEntityIfRequired(dbContext, enumEntity, existingDbEntity);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void AddEnumEntityIfRequired(ApplicationDbContext dbContext, TEnumEntity enumEntity, TEnumEntity? existingDbEntity)
    {
        if (existingDbEntity is null)
        {
            _logger.LogInformation("Enum found in code {EnumName} does not exist in the databse, so creating it.", enumEntity.Name);
            dbContext.Add(enumEntity);
            return;
        }

        if (enumEntity.Name != existingDbEntity.Name)
        {
            throw new EnumInitializationException("Mismatch between enum in code and db:\n" +
                $"Id: {enumEntity.Id}\n" +
                $"Name in code: {enumEntity.Name}\n" +
                $"Name in db: {existingDbEntity.Name}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
