using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private const string AllProductsCacheKey = "products:all";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

        private readonly IProductRepository _repo;
        private readonly ICacheService _cache;
        private readonly IPerformanceMetrics _metrics;

        public ProductService(IProductRepository repo, ICacheService cache, IPerformanceMetrics metrics)
        {
            _repo = repo;
            _cache = cache;
            _metrics = metrics;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            var cachedProducts = await _cache.GetAsync<IEnumerable<ProductResponseDto>>(AllProductsCacheKey);
            if (cachedProducts != null)
            {
                _metrics.RecordCacheHit();
                return cachedProducts;
            }

            _metrics.RecordCacheMiss();
            var products = await _repo.GetAllAsync();
            var response = products.Select(MapToResponse).ToList();

            await _cache.SetAsync(AllProductsCacheKey, response, CacheExpiration);

            return response;
        }

        public async Task<ProductResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = GetProductCacheKey(id);
            var cachedProduct = await _cache.GetAsync<ProductResponseDto>(cacheKey);
            if (cachedProduct != null)
            {
                _metrics.RecordCacheHit();
                return cachedProduct;
            }

            _metrics.RecordCacheMiss();
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return null;

            var response = MapToResponse(product);
            await _cache.SetAsync(cacheKey, response, CacheExpiration);

            return response;
        }

        public async Task<ProductResponseDto> CreateAsync(ProductDto dto)
        {
            Validate(dto);

            var product = new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price
            };

            var created = await _repo.AddAsync(product);
            await InvalidateProductCacheAsync(created.Id);

            return MapToResponse(created);
        }

        public async Task<ProductResponseDto?> UpdateAsync(int id, ProductDto dto)
        {
            Validate(dto);

            var updated = await _repo.UpdateAsync(id, new Product
            {
                Name = dto.Name.Trim(),
                Price = dto.Price
            });

            if (updated == null)
                return null;

            await InvalidateProductCacheAsync(id);

            return MapToResponse(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (deleted)
                await InvalidateProductCacheAsync(id);

            return deleted;
        }

        private static void Validate(ProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required");

            if (dto.Price <= 0)
                throw new ArgumentException("Price must be greater than zero");
        }

        private static ProductResponseDto MapToResponse(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price
            };
        }

        private static string GetProductCacheKey(int id) => $"products:{id}";

        private async Task InvalidateProductCacheAsync(int productId)
        {
            await _cache.RemoveAsync(AllProductsCacheKey);
            await _cache.RemoveAsync(GetProductCacheKey(productId));
        }
    }
}
