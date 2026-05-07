using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Caching;
using Infrastructure.Data;
using Infrastructure.Monitoring;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext - SQL Server (uses DefaultConnection from appsettings.json)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories / application services
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            // Monitoring / metrics - in-memory singleton
            services.AddSingleton<IPerformanceMetrics, PerformanceMetrics>();

            // Distributed cache: register an IDistributedCache implementation (in-memory fallback)
            // This allows RedisCacheService to work with either Redis (if configured) or the in-memory provider.
            services.AddDistributedMemoryCache();

            // Cache service wrapper that uses IDistributedCache (RedisCacheService exists in Infrastructure.Caching)
            services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}