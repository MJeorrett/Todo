using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;

namespace Todo.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<TodoEntity> Todos { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
