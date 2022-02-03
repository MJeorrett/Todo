using NodaTime;
using Todo.Application.Common.Interfaces;

namespace Todo.Infrastructure.DateTimes;

public class DateTimeService : IDateTimeService
{
    public ZonedDateTime Now => SystemClock.Instance.GetCurrentInstant().InUtc();
}
