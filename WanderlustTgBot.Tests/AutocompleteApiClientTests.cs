namespace WanderlustTgBot.Tests;

public class AutocompleteApiClientTests
{
    private readonly MockHttpMessageHandler _handlerMock = new();

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnDeparturePoint()
    {
        // Arrange
        var httpContent = JsonContent.Create(new object[] { new {
            code = "LAX",
            name = "Los Angeles",
            country_code = "US",
            country_name = "United States"
        }});

        var httpClientMock = CreateHttpClientMock(HttpStatusCode.OK, httpContent);

        var expected = new DeparturePoint
        {
            Code = "LAX",
            Name = "Los Angeles",
            CountryCode = "US",
            CountryName = "United States"
        };

        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetLocaleByCityAsync("Los Angeles");

        // Assert
        result
            .Should().NotBeNull()
            .And.BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenEmptyValueIsPassed()
    {
        // Arrange
        var httpClientMock = CreateHttpClientMock(HttpStatusCode.NotFound);
        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetLocaleByCityAsync(string.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenBadCharactersIsPassed()
    {
        // Arrange
        var httpClientMock = CreateHttpClientMock(HttpStatusCode.BadRequest);
        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetLocaleByCityAsync("?");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenDataNotFound()
    {
        // Arrange
        var httpClientMock = CreateHttpClientMock(
            HttpStatusCode.OK,
            JsonContent.Create(Array.Empty<object>()));
        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetLocaleByCityAsync("test");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldThrowException_WhenAutocompleteApiIsUnavailable()
    {
        // Arrange
        var httpClientMock = CreateHttpClientMock(HttpStatusCode.InternalServerError);
        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var act = () => sut.GetLocaleByCityAsync("test");

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private HttpClient CreateHttpClientMock(
        HttpStatusCode httpStatusCode,
        HttpContent? httpContent = null)
    {
        _handlerMock
            .When("https://autocomplete.travelpayouts.com/*")
            .Respond(httpStatusCode, httpContent);

        var httpClient = _handlerMock.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://autocomplete.travelpayouts.com");

        return httpClient;
    }
}