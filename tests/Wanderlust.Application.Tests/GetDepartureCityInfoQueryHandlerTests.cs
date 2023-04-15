using CityDto = Wanderlust.Application.Departure.Dtos.CityDto;

namespace Wanderlust.Application.Tests;

public class GetDepartureCityInfoQueryHandlerTests
{
    private readonly Mock<ILogger<GetDepartureCityInfoQueryHandler>> _loggerMock;
    private readonly Mock<IAutocompleteApiClient> _clientMock;
    private readonly Mock<ICitiesRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IFixture _fixture;

    public GetDepartureCityInfoQueryHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetDepartureCityInfoQueryHandler>>(MockBehavior.Strict);
        _clientMock = new Mock<IAutocompleteApiClient>(MockBehavior.Strict);
        _repositoryMock = new Mock<ICitiesRepository>(MockBehavior.Strict);
        _mapperMock = new Mock<IMapper>(MockBehavior.Strict);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Should_return_city_dto()
    {
        // Arrange
        _clientMock
            .Setup(c => c.GetCityCodeByNameAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(_fixture.Create<string>());

        var city = _fixture.Create<City>();
        _repositoryMock
            .Setup(r => r.GetCityByCodeAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(city);

        var cityDto = _fixture.Create<CityDto>();
        _mapperMock
            .Setup(m => m.Map<CityDto>(city))
            .Returns(cityDto);

        var sut = CreateHandler();

        // Act
        var result = await sut.Handle(
            _fixture.Create<GetDepartureCityInfoQuery>(),
            CancellationToken.None);

        // Assert
        result.Should().BeSameAs(cityDto);

        _mapperMock.Verify(m => m.Map<CityDto>(
            It.Is<City>(c => c == city)));
    }

    [Fact]
    public async Task Should_return_null_if_city_does_not_exist()
    {
        // Arrange
        _clientMock
            .Setup(c => c.GetCityCodeByNameAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(string.Empty);

        var sut = CreateHandler();

        // Act
        var result = await sut.Handle(
            _fixture.Create<GetDepartureCityInfoQuery>(),
            CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_return_null_if_city_not_found_in_database()
    {
        // Arrange
        _clientMock
            .Setup(c => c.GetCityCodeByNameAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(_fixture.Create<string>());

        _repositoryMock
            .Setup(r => r.GetCityByCodeAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync(null as City);

        _loggerMock.Setup();

        var sut = CreateHandler();

        // Act
        var result = await sut.Handle(
            _fixture.Create<GetDepartureCityInfoQuery>(),
            CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _loggerMock.Verify(LogLevel.Warning, Times.Once());
    }

    private GetDepartureCityInfoQueryHandler CreateHandler()
         => new(
            _loggerMock.Object,
            _clientMock.Object,
            _repositoryMock.Object,
            _mapperMock.Object
        );
}