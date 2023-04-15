using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Utf8Json;
using Wanderlust.Core.Domain;
using Wanderlust.Application.Common.Interfaces;

namespace Wanderlust.Infrastructure.Persistence;

public class DepartureBoardCache : IDepartureBoardsCache
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IConfiguration _configuration;

    public DepartureBoardCache(
        IConnectionMultiplexer redisConnection,
        IConfiguration configuration)
    {
        _redisConnection = redisConnection;
        _configuration = configuration;
    }

    public async Task AddDepartureBoardsAsync(
        IEnumerable<DepartureBoard> departureBoards,
        GeoRegion region)
    {
        var tasks = new List<Task>();

        var expiry = TimeSpan.FromMinutes(
            _configuration.GetValue<int>("DepartureBoardsCacheExpirationInMinutes"));

        foreach (var departureBoard in departureBoards)
        {
            var key = GetKey(departureBoard.DepartureCity.Code, region, departureBoard.Date);
            var value = JsonSerializer.Serialize(departureBoard);

            tasks.Add(_redisConnection.GetDatabase()
                .StringSetAsync(key, value, expiry));
        }

        await Task.WhenAll(tasks);
    }

    public async Task<DepartureBoard?> GetDepartureBoardAsync(
        string departureCode,
        GeoRegion region,
        DateOnly date)
    {
        var key = GetKey(departureCode, region, date);
        var value = await _redisConnection.GetDatabase().StringGetAsync(key);

        return !value.IsNullOrEmpty
            ? JsonSerializer.Deserialize<DepartureBoard>(value.ToString())
            : null;
    }

    private static string GetKey(string departureCode, GeoRegion region, DateOnly date)
        => string.Join(':', departureCode, region, date);
}
