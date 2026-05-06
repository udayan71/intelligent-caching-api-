using System.Diagnostics;
using Application.Interfaces.Services;

namespace intelligent_caching_api.Middleware
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IPerformanceMetrics metrics)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Response-Time-ms"] = stopwatch.Elapsed.TotalMilliseconds.ToString("0.##");
                return Task.CompletedTask;
            });

            await _next(context);

            stopwatch.Stop();

            var endpoint = $"{context.Request.Method} {context.Request.Path}";
            metrics.RecordResponseTime(endpoint, stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
