using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Interfaces.Settings;

namespace CareerFlow.Infrastructure.External.Cache;

public class RedisCacheService : ICacheService, IDisposable
{
    private readonly IConnectionMultiplexer? _connection;
    private readonly IDatabase? _db;
    private readonly RedisSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly bool _available;

    public RedisCacheService(IOptions<RedisSettings> settings, ILogger<RedisCacheService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (!_settings.Enabled)
        {
            _logger.LogWarning("⚡ Redis desabilitado. Cache não será utilizado.");
            _available = false;
            return;
        }

        try
        {
            _connection = ConnectionMultiplexer.Connect(_settings.ConnectionString);
            _db = _connection.GetDatabase();
            _available = true;
            _logger.LogInformation("✅ Redis conectado: {Host}", _settings.ConnectionString.Split(',')[0]);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Redis não disponível. Cache operando em modo degradado.");
            _available = false;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        if (!_available || _db == null) return null;

        var value = await _db.StringGetAsync(GetKey(key));
        if (!value.HasValue) return default;

        var json = (string?)value;
        return json != null ? JsonSerializer.Deserialize<T>(json) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) where T : class
    {
        if (!_available || _db == null) return;

        var serialized = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(GetKey(key), serialized, expiration ?? GetDefaultExpiration());
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (!_available || _db == null) return;
        await _db.KeyDeleteAsync(GetKey(key));
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        if (!_available || _db == null) return false;
        return await _db.KeyExistsAsync(GetKey(key));
    }

    public async Task<T?> GetOrAddAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null, CancellationToken ct = default) where T : class
    {
        if (!_available || _db == null)
            return await factory();

        var cached = await GetAsync<T>(key, ct);
        if (cached != null) return cached;

        var value = await factory();
        if (value != null)
            await SetAsync(key, value, expiration, ct);

        return value;
    }

    public async Task InvalidateByPatternAsync(string pattern, CancellationToken ct = default)
    {
        if (!_available || _connection == null) return;

        try
        {
            var server = _connection.GetServer(_connection.GetEndPoints().First());
            var keys = server.Keys(pattern: GetKey(pattern + "*")).ToArray();

            if (keys.Length > 0)
            {
                await _db!.KeyDeleteAsync(keys);
                _logger.LogDebug("🧹 Cache invalidado para padrão: {Pattern} ({Count} chaves)", pattern, keys.Length);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha ao invalidar cache por padrão: {Pattern}", pattern);
        }
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private string GetKey(string key) => $"{_settings.InstanceName}{key}";

    private TimeSpan GetDefaultExpiration() =>
        TimeSpan.FromMinutes(_settings.DefaultCacheDurationMinutes);
}
