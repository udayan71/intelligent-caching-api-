using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IPerformanceMetrics
    {
        void RecordCacheHit();
        void RecordCacheMiss();
        void RecordResponseTime(string endpoint, double elapsedMilliseconds);
        CachePerformanceDto GetSnapshot();
    }
}
