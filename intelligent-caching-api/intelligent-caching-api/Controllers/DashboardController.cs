using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace intelligent_caching_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IPerformanceMetrics _metrics;

        public DashboardController(IPerformanceMetrics metrics)
        {
            _metrics = metrics;
        }

        [HttpGet("performance")]
        public IActionResult GetPerformance()
        {
            return Ok(_metrics.GetSnapshot());
        }
    }
}
