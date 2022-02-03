using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;

namespace Todo.Infrastructure.Persistence.EnityConfigurations;

public class ValueConverters
{
    public static ValueConverter<ZonedDateTime, DateTime> ZonedDateTimeValueConverter = new(
            value => value.ToDateTimeUtc(),
            value => Instant.FromDateTimeUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc))
                .InUtc());

    public static ValueConverter<ZonedDateTime?, DateTime?> NullableZonedDateTimeValueConverter = new(
            value => value == null ? null : value.Value.ToDateTimeUtc(),
            value => value == null ? null : Instant.FromDateTimeUtc(DateTime.SpecifyKind(value.Value, DateTimeKind.Utc))
                .InUtc());
}
