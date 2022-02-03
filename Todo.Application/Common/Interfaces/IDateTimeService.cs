using NodaTime;

namespace Todo.Application.Common.Interfaces;

public interface IDateTimeService
{
    ZonedDateTime Now { get; }
}
