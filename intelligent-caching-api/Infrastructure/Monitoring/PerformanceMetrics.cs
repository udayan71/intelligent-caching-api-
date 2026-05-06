using System.Collections.Concurrent;
using Application.DTOs;
using Application.Interfaces.Services;

namespace Infrastructure.Monitoring
{
    public class PerformanceMetrics : IPerformanceMetrics
    {
        private readonly ConcurrentDictionary<string, EndpointStats> _endpointStats = new();
        private long _cacheHits;
        private long _cacheMisses;
        private long _totalResponseTicks;
        private long _totalResponses;

        public void RecordCacheHit()
        {
            Interlocked.Increment(ref _cacheHits);
        }

        public void RecordCacheMiss()
        {
            Interlocked.Increment(ref _cacheMisses);
        }

        public void RecordResponseTime(string endpoint, double elapsedMilliseconds)
        {
            Interlocked.Increment(ref _totalResponses);
            Interlocked.Add(ref _totalResponseTicks, TimeSpan.FromMilliseconds(elapsedMilliseconds).Ticks);

            var stats = _endpointStats.GetOrAdd(endpoint, _ => new EndpointStats());
            stats.Record(elapsedMilliseconds);
        }

        public CachePerformanceDto GetSnapshot()
        {
            var hits = Interlocked.Read(ref _cacheHits);
            var misses = Interlocked.Read(ref _cacheMisses);
            var totalRequests = hits + misses;
            var totalResponses = Interlocked.Read(ref _totalResponses);
            var totalResponseTicks = Interlocked.Read(ref _totalResponseTicks);

            return new CachePerformanceDto
            {
                TotalRequests = totalRequests,
                CacheHits = hits,
                CacheMisses = misses,
                HitRate = totalRequests == 0 ? 0 : Math.Round((double)hits / totalRequests * 100, 2),
                AverageResponseTimeMs = totalResponses == 0
                    ? 0
                    : Math.Round(TimeSpan.FromTicks(totalResponseTicks / totalResponses).TotalMilliseconds, 2),
                Endpoints = _endpointStats
                    .Select(entry => entry.Value.ToDto(entry.Key))
                    .OrderByDescending(metric => metric.Count)
                    .ToList()
            };
        }

        private class EndpointStats
        {
            private readonly object _lock = new();
            private long _count;
            private double _totalMilliseconds;
            private double _fastestMilliseconds = double.MaxValue;
            private double _slowestMilliseconds;

            public void Record(double elapsedMilliseconds)
            {
                lock (_lock)
                {
                    _count++;
                    _totalMilliseconds += elapsedMilliseconds;
                    _fastestMilliseconds = Math.Min(_fastestMilliseconds, elapsedMilliseconds);
                    _slowestMilliseconds = Math.Max(_slowestMilliseconds, elapsedMilliseconds);
                }
            }

            public EndpointMetricDto ToDto(string endpoint)
            {
                lock (_lock)
                {
                    return new EndpointMetricDto
                    {
                        Endpoint = endpoint,
                        Count = _count,
                        AverageResponseTimeMs = _count == 0 ? 0 : Math.Round(_totalMilliseconds / _count, 2),
                        FastestResponseTimeMs = _count == 0 ? 0 : Math.Round(_fastestMilliseconds, 2),
                        SlowestResponseTimeMs = Math.Round(_slowestMilliseconds, 2)
                    };
                }
            }
        }
    }
}
