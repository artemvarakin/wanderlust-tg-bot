namespace Wanderlust.Application.Tests;

public class FindVisaFreeZoneFlightsQueryHandlerTests
{
    private readonly Mock<IDepartureBoardsCache> _cacheMock;
    private readonly Mock<IDepartureBoardsProvider> _providerMock;
    private readonly Mock<IMapper> _mapperMock;
    private static readonly IFixture _fixture = new Fixture();

    public FindVisaFreeZoneFlightsQueryHandlerTests()
    {
        _cacheMock = new Mock<IDepartureBoardsCache>(MockBehavior.Strict);
        _providerMock = new Mock<IDepartureBoardsProvider>(MockBehavior.Strict);
        _mapperMock = new Mock<IMapper>(MockBehavior.Strict);

        _fixture.Customize<DateOnly>(c => c.FromFactory<DateTime>(DateOnly.FromDateTime));
    }

    [Fact]
    public async Task Should_return_departure_board_if_it_is_cached()
    {
        // Arrange
        _mapperMock
            .Setup(m => m.Map<GeoRegion>(It.IsAny<GeoRegionDto>()))
            .Returns(_fixture.Create<GeoRegion>());

        var departureBoard = _fixture.Create<DepartureBoard>();
        _cacheMock
            .Setup(c => c.GetDepartureBoardAsync(It.IsAny<string>(), It.IsAny<GeoRegion>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(departureBoard);

        var departureBoardDto = _fixture.Create<DepartureBoardDto>();
        _mapperMock
            .Setup(m => m.Map<DepartureBoardDto>(departureBoard))
            .Returns(departureBoardDto);

        var sut = CreateHandler();

        // Act
        var result = await sut.Handle(_fixture.Create<FindVisaFreeZoneFlightsQuery>(), CancellationToken.None);

        // Assert
        result.Should().BeSameAs(departureBoardDto);

        _providerMock.Verify(
            p => p.GetDepartureBoardsAsync(
                It.IsAny<string>(),
                It.IsAny<GeoRegion>(),
                CancellationToken.None),
            Times.Never);

        _cacheMock.Verify(
            p => p.AddDepartureBoardsAsync(
                _fixture.CreateMany<DepartureBoard>(),
                It.IsAny<GeoRegion>()),
            Times.Never);
    }

    [Theory]
    [MemberData(nameof(GetDepartureBoardsTestData))]
    public async Task Should_return_departure_board_if_it_not_cached(
        IEnumerable<DepartureBoard> departureBoards, DateOnly requestedDate)
    {
        // Arrange
        _mapperMock
            .Setup(m => m.Map<GeoRegion>(It.IsAny<GeoRegionDto>()))
            .Returns(_fixture.Create<GeoRegion>());

        _cacheMock
            .Setup(c => c.GetDepartureBoardAsync(It.IsAny<string>(), It.IsAny<GeoRegion>(), It.IsAny<DateOnly>()))
            .ReturnsAsync(null as DepartureBoard);

        _providerMock
            .Setup(p => p.GetDepartureBoardsAsync(It.IsAny<string>(), It.IsAny<GeoRegion>(), CancellationToken.None))
            .ReturnsAsync(departureBoards);

        _cacheMock
            .Setup(c => c.AddDepartureBoardsAsync(departureBoards, It.IsAny<GeoRegion>()))
            .Returns(Task.CompletedTask);

        var departureBoardDto = _fixture.Create<DepartureBoardDto>();
        _mapperMock
            .Setup(m => m.Map<DepartureBoardDto>(It.IsAny<DepartureBoard>()))
            .Returns(departureBoardDto);

        var sut = CreateHandler();

        // Act
        var result = await sut.Handle(
            _fixture.Create<FindVisaFreeZoneFlightsQuery>() with { Date = requestedDate },
            CancellationToken.None);

        // Assert
        result.Should()
            .BeSameAs(departureBoardDto);

        _cacheMock.Verify(
            p => p.AddDepartureBoardsAsync(
                departureBoards,
                It.IsAny<GeoRegion>()),
            Times.Once);

        _mapperMock.Verify(m => m.Map<DepartureBoardDto>(
            It.Is<DepartureBoard>(d => d.Date == requestedDate)));
    }

    public static IEnumerable<object[]> GetDepartureBoardsTestData()
    {
        var fixture = new Fixture();
        fixture.Customize<DateOnly>(c => c.FromFactory<DateTime>(DateOnly.FromDateTime));
        var date = DateOnly.FromDateTime(DateTime.Today);
        yield return new object[]
        {
            new DepartureBoard[]
            {
                fixture.Create<DepartureBoard>() with {Date = date},
                fixture.Create<DepartureBoard>() with {Date = date.AddDays(1)},
                fixture.Create<DepartureBoard>() with {Date = date.AddDays(2)},
            },
            date
        };
    }

    private FindVisaFreeZoneFlightsQueryHandler CreateHandler()
        => new(
            _mapperMock.Object,
            _cacheMock.Object,
            _providerMock.Object
        );
}