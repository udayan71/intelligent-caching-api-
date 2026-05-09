using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Caching;
using Infrastructure.Data;
using Infrastructure.Monitoring;
using Infrastructure.Repositories;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "IntelligentCachingApi:";
            });

            services.AddMemoryCache();
            services.AddScoped<RedisCacheService>();
            services.AddScoped<ICacheService, FaultTolerantCacheService>();
            services.AddSingleton<IPerformanceMetrics, PerformanceMetrics>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
