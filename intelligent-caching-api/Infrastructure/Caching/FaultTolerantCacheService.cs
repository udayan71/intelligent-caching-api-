using Application.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Caching
{
    public class FaultTolerantCacheService : ICacheService
    {
        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
        private readonly RedisCacheService _redis;
        private readonly IMemoryCache _memory;
        private readonly ILogger<FaultTolerantCacheService> _logger;

        // Short in-memory fallback TTL to avoid long-lived divergence from Redis.
        private readonly TimeSpan _memoryFallbackExpiration = TimeSpan.FromMinutes(5);

        public FaultTolerantCacheService(
            RedisCacheService redis,
            IMemoryCache memory,
            ILogger<FaultTolerantCacheService> logger)
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            // Try Redis first, but swallow any Redis exceptions and fall back to memory cache.
            try
            {
                var value = await _redis.GetAsync<T>(key, cancellationToken);
                if (value != null)
                    return value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis GET failed for key {Key}. Falling back to in-memory cache.", key);
            }

            try
            {
                if (_memory.TryGetValue(key, out T memValue))
                    return memValue;
            }
            catch (Exception ex)
            {
                // Extremely unlikely, but don't let memory cache issues bubble up.
                _logger.LogError(ex, "In-memory GET failed for key {Key}.", key);
            }

            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
        {
            // Try to update Redis; if it fails, keep going and persist to memory fallback.
            try
            {
                await _redis.SetAsync(key, value, expiration, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis SET failed for key {Key}. Storing in in-memory fallback.", key);
            }

            try
            {
                // Store in in-memory cache as fallback (use shorter TTL than primary cache to limit divergence).
                var memOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _memoryFallbackExpiration < expiration
                        ? _memoryFallbackExpiration
                        : expiration
                };
                _memory.Set(key, value, memOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In-memory SET failed for key {Key}.", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _redis.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}. Removing from in-memory fallback.", key);
            }

            try
            {
                _memory.Remove(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "In-memory REMOVE failed for key {Key}.", key);
            }
        }
    }
}
