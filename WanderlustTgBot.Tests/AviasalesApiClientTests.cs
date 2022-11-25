namespace WanderlustTgBot.Tests;

public class AviasalesApiClientTests
{
    private readonly MockHttpMessageHandler _handlerMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private const string _aviasalesApiUrl = "https://api.travelpayouts.com";
    private const string _matchUrl = $"{_aviasalesApiUrl}/*";
    private const string _firstDeparture = "2023-01-15T12:00:00+03:00";

    public AviasalesApiClientTests()
    {
        _handlerMock = new MockHttpMessageHandler();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>(MockBehavior.Strict);
    }

    [Theory]
    [InlineData(_firstDeparture, "2023-02-15T11:58:00+03:00", 2)]
    [InlineData(_firstDeparture, "2023-02-15T12:00:00+03:00", 1)]
    [InlineData(_firstDeparture, "2023-02-01T12:00:00+03:00", 2)]
    public async Task GetFlightsForMonthAsync_ShouldReturnFlightsForOneMonthFurther(
        string firstFlightDeparture,
        string secondFlightDeparture,
        int expectedFlightsCount)
    {
        // Arrange
        var currentMonth = JsonContent.Create(new
        {
            data = new object[] {
                new { departure_at = firstFlightDeparture }
            }
        });

        var nextMonth = JsonContent.Create(new
        {
            data = new object[] {
                new { departure_at = secondFlightDeparture }
            }
        });

        _handlerMock.Expect(_matchUrl)
            .Respond(HttpStatusCode.OK, currentMonth);

        _handlerMock.Expect(_matchUrl)
            .Respond(HttpStatusCode.OK, nextMonth);

        var client = _handlerMock.ToHttpClient();
        client.BaseAddress = new Uri(_aviasalesApiUrl);

        _dateTimeProviderMock
            .Setup(p => p.UtcNow)
            .Returns(DateTime.Parse(_firstDeparture).AddMinutes(-1));

        var sut = new AviasalesApiClient(client, _dateTimeProviderMock.Object);

        // Act
        var result = await sut.GetFlightsForMonthAsync("LED", "BUD");

        // Assert
        result.Should().HaveCount(expectedFlightsCount);
    }
}