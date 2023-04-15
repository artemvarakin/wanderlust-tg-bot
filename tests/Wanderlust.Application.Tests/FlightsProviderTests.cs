namespace Wanderlust.Application.Tests;

public class FlightsProviderTests
{
    private readonly Mock<ILogger<FlightsProvider>> _loggerMock;
    private readonly Mock<IAviasalesApiClient> _clientMock;
    private readonly Mock<IVisaFreeDirectionsRepository> _directionsRepositoryMock;
    private readonly Mock<ICitiesRepository> _citiesRepositoryMock;
    private static readonly Fixture _fixture = new();

    public FlightsProviderTests()
    {
        _loggerMock = new Mock<ILogger<FlightsProvider>>(MockBehavior.Strict);
        _clientMock = new Mock<IAviasalesApiClient>(MockBehavior.Strict);
        _directionsRepositoryMock = new Mock<IVisaFreeDirectionsRepository>(MockBehavior.Strict);
        _citiesRepositoryMock = new Mock<ICitiesRepository>(MockBehavior.Strict);
    }

    [Fact]
    public async Task GetFlightsForMonthAsync_should_throw_error_if_no_direction_codes_found()
    {
        // Arrange
        _directionsRepositoryMock
            .Setup(r => r.GetDirectionCodesByRegionAsync(It.IsAny<GeoRegion>(), CancellationToken.None))
            .ReturnsAsync(Enumerable.Empty<string>);

        var sut = CreateProvider();

        // Act
        var act = async () => await sut.GetFlightsForMonthAsync(
            GetCity(), _fixture.Create<GeoRegion>(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetFlightsForMonthAsync_should_return_empty_list_if_no_flights_found()
    {
        // Arrange
        _directionsRepositoryMock
            .Setup(r => r.GetDirectionCodesByRegionAsync(It.IsAny<GeoRegion>(), CancellationToken.None))
            .ReturnsAsync(_fixture.CreateMany<string>());

        _clientMock
            .Setup(c => c.GetFlightsForMonthAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(Enumerable.Empty<AviasalesFlight>);

        _loggerMock.Setup();

        var sut = CreateProvider();

        // Act
        var result = await sut.GetFlightsForMonthAsync(
            GetCity(), _fixture.Create<GeoRegion>(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();

        _loggerMock.Verify(LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task GetFlightsForMonthAsync_should_throw_error_if_no_cities_found()
    {
        // Arrange
        _directionsRepositoryMock
            .Setup(r => r.GetDirectionCodesByRegionAsync(It.IsAny<GeoRegion>(), CancellationToken.None))
            .ReturnsAsync(_fixture.CreateMany<string>());

        _clientMock
            .Setup(c => c.GetFlightsForMonthAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(_fixture.CreateMany<AviasalesFlight>());

        _citiesRepositoryMock
            .Setup(r => r.GetCitiesByCodesAsync(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
            .ReturnsAsync(Enumerable.Empty<City>);

        var sut = CreateProvider();

        // Act
        var act = async () => await sut.GetFlightsForMonthAsync(
            GetCity(), _fixture.Create<GeoRegion>(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    private FlightsProvider CreateProvider()
    {
        return new FlightsProvider(
            _loggerMock.Object,
            _clientMock.Object,
            _directionsRepositoryMock.Object,
            _citiesRepositoryMock.Object
        );
    }

    private static City GetCity() => _fixture.Build<City>().Create();
}