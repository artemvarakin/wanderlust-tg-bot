namespace WanderlustTgBot.Tests;

public class AutocompleteApiClientTests
{
    private readonly MockHttpMessageHandler _handlerMock = new();

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnDepartureCode()
    {
        // Arrange
        const string code = "LAX";
        var httpContent = JsonContent.Create(new object[] { new { code } });

        var httpClientMock = CreateHttpClientMock(HttpStatusCode.OK, httpContent);

        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetDepartureCodeByNameAsync("Los Angeles");

        // Assert
        result
            .Should().NotBeNull()
            .And.BeEquivalentTo(code);
    }

    [Fact]
    public async Task GetCityCodeAsync_ShouldReturnNull_WhenEmptyValueIsPassed()
    {
        // Arrange
        var httpClientMock = CreateHttpClientMock(HttpStatusCode.NotFound);
        var sut = new AutocompleteApiClient(httpClientMock);

        // Act
        var result = await sut.GetDepartureCodeByNameAsync(string.Empty);

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
        var result = await sut.GetDepartureCodeByNameAsync("?");

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
        var result = await sut.GetDepartureCodeByNameAsync("test");

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
        var act = () => sut.GetDepartureCodeByNameAsync("test");

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