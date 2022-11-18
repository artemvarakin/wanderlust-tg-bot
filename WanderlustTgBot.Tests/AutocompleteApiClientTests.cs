namespace WanderlustTgBot.Tests;

public class AutocompleteApiClientTests
{
    private readonly AutocompleteApiClient _sut;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly MockHttpMessageHandler _handlerMock = new();
    private const string _autocompleteApiUrl = "https://autocomplete.travelpayouts.com";
    private const string _matchUrl = $"{_autocompleteApiUrl}/*";

    public AutocompleteApiClientTests()
    {
        _sut = new AutocompleteApiClient(_httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnDeparturePoint_WhenValidValuePassed()
    {
        // Arrange
        _handlerMock
            .When(_matchUrl)
            .Respond(HttpStatusCode.OK,
                JsonContent.Create(new object[] { new {
                    code = "LAX",
                    name = "Los Angeles",
                    country_code = "US",
                    country_name = "United States"
                }})
            );

        SetupHttpClientFactoryMock(_handlerMock);

        var expected = new DeparturePoint
        {
            Code = "LAX",
            Name = "Los Angeles",
            CountryCode = "US",
            CountryName = "United States"
        };

        // Act
        var result = await _sut.GetLocaleByCityAsync("Los Angeles");

        // Assert
        result
            .Should().NotBeNull()
            .And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenEmptyValueIsPassed()
    {
        // Arrange
        _handlerMock
            .When(_matchUrl)
            .Respond(HttpStatusCode.NotFound);

        SetupHttpClientFactoryMock(_handlerMock);

        // Act
        var result = await _sut.GetLocaleByCityAsync(string.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenBadCharactersIsPassed()
    {
        // Arrange
        _handlerMock
            .When(_matchUrl)
            .Respond(HttpStatusCode.BadRequest);

        SetupHttpClientFactoryMock(_handlerMock);

        // Act
        var result = await _sut.GetLocaleByCityAsync("?");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenDataNotFound()
    {
        // Arrange
        _handlerMock
            .When(_matchUrl)
            .Respond(
                HttpStatusCode.OK,
                JsonContent.Create(Array.Empty<object>())
            );

        SetupHttpClientFactoryMock(_handlerMock);

        // Act
        var result = await _sut.GetLocaleByCityAsync("test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldThrowException_WhenAutocompleteApiIsUnavailable()
    {
        // Arrange
        _handlerMock
            .When(_matchUrl)
            .Respond(HttpStatusCode.InternalServerError);

        SetupHttpClientFactoryMock(_handlerMock);

        // Act
        var act = () => _sut.GetLocaleByCityAsync("test");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private void SetupHttpClientFactoryMock(MockHttpMessageHandler handlerMock)
    {
        _httpClientFactoryMock.Setup(x => x.CreateClient(nameof(IAutocompleteApiClient)))
            .Returns(new HttpClient(handlerMock)
            {
                BaseAddress = new Uri(_autocompleteApiUrl)
            });
    }
}