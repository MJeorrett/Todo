using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Todo.Application.Common.Interfaces;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<TodoEntity> Todos { get; init; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
