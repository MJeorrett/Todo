using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Persistence.EnityConfigurations;

public class TodoEntityConfiguration : IEntityTypeConfiguration<TodoEntity>
{
    public void Configure(EntityTypeBuilder<TodoEntity> builder)
    {
        builder.ToTable("Todo");
        builder.HasKey(_ => _.Id);

        builder.Property(_ => _.Id)
            .HasColumnName("TodoId");

        builder.Property(_ => _.CreatedAt)
            .HasConversion(ValueConverters.ZonedDateTimeValueConverter);

        builder.Property(_ => _.LastUpdatedAt)
            .HasConversion(ValueConverters.NullableZonedDateTimeValueConverter);

        builder.Property(_ => _.Title)
            .HasColumnType("varchar(256)");
    }
}
