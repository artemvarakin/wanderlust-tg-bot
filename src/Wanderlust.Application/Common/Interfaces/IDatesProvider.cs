namespace Wanderlust.Application.Common.Interfaces;

public interface IDatesProvider
{
    IEnumerable<DateOnly> GetDatesOneMonthAhead(DateOnly startDate);
}