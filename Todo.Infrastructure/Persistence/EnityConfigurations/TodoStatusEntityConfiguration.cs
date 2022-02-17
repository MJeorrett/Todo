using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Enums;

namespace Todo.Infrastructure.Persistence.EnityConfigurations;

public class TodoStatusEntityConfiguration : IEntityTypeConfiguration<TodoStatusEntity>
{
    public void Configure(EntityTypeBuilder<TodoStatusEntity> builder)
    {
        builder.ToTable("TodoStatus");
        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.Id)
            .HasColumnName("TodoStatusId");

        builder.Property(_ => _.Name)
            .HasColumnType("varchar(64)");
    }
}
