namespace Wanderlust.Application.Tests;

public class DatesProviderTests
{
    private readonly IDatesProvider _sut;

    public DatesProviderTests()
    {
        _sut = new DatesProvider();
    }

    [Theory]
    [MemberData(nameof(GetDaysAmountTestData))]
    public void GetDatesOneMonthAhead_should_return_days_in_amount_of_one_month_theory(
        DateOnly startDate, int expectedDaysCount)
    {
        // Act
        var result = _sut.GetDatesOneMonthAhead(startDate);

        // Assert
        result.Should()
            .HaveCount(expectedDaysCount)
            .Equals(new DateOnly());
    }

    public static IEnumerable<object[]> GetDaysAmountTestData()
    {
        yield return new object[] { new DateOnly(2023, 6, 1), 30 };
        yield return new object[] { new DateOnly(2023, 6, 30), 30 };
        yield return new object[] { new DateOnly(2023, 7, 1), 31 };
        yield return new object[] { new DateOnly(2023, 7, 31), 31 };
        yield return new object[] { new DateOnly(2023, 1, 31), 28 };
        yield return new object[] { new DateOnly(2024, 1, 31), 29 };
    }
}