using Wanderlust.Application.Common.Interfaces;

namespace Wanderlust.Application.Common.Services;

public class DatesProvider : IDatesProvider
{
    public IEnumerable<DateOnly> GetDatesOneMonthAhead(DateOnly startDate)
    {
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var dates = new List<DateOnly>();

        while (startDate <= endDate)
        {
            dates.Add(startDate);
            startDate = startDate.AddDays(1);
        }

        return dates;
    }
}