using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Common;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Infrastructure.Identity;

namespace Todo.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public DbSet<TodoEntity> Todos { get; init; } = null!;
    public DbSet<TodoStatusEntity> TodoStatuses { get; init; } = null!;

    private readonly IDateTimeService _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDateTimeService dateTimeService,
        ICurrentUserService currentUserService) :
        base(options)
    {
        _dateTimeService = dateTimeService;
        _currentUserService = currentUserService;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetAuditFields();

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetAuditFields();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = _dateTimeService.Now;
                    entry.Entity.CreatedBy = _currentUserService.GetUserId() ?? "";
                    break;

                case EntityState.Modified:
                    entry.Entity.LastUpdatedAt = _dateTimeService.Now;
                    entry.Entity.LastUpdatedBy = _currentUserService.GetUserId() ?? "";
                    break;
            }
        }
    }
}
