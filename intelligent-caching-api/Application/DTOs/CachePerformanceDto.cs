namespace Application.DTOs
{
    public class CachePerformanceDto
    {
        public long TotalRequests { get; set; }
        public long CacheHits { get; set; }
        public long CacheMisses { get; set; }
        public double HitRate { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public IReadOnlyCollection<EndpointMetricDto> Endpoints { get; set; } = [];
    }

    public class EndpointMetricDto
    {
        public string Endpoint { get; set; } = string.Empty;
        public long Count { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public double FastestResponseTimeMs { get; set; }
        public double SlowestResponseTimeMs { get; set; }
    }
}
