using Application.Common.Pagination;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(ProductQueryParams queryParams)
        {
            var query = _context.Products.AsQueryable();

            // FILTERING

            if (!string.IsNullOrWhiteSpace(queryParams.Category))
            {
                query = query.Where(product =>
                    product.Category == queryParams.Category);
            }

            if (queryParams.MinPrice.HasValue)
            {
                query = query.Where(product =>
                    product.Price >= queryParams.MinPrice.Value);
            }

            if (queryParams.MaxPrice.HasValue)
            {
                query = query.Where(product =>
                    product.Price <= queryParams.MaxPrice.Value);
            }

            // PAGINATION

            query = query
                .OrderBy(product => product.Id)
                .Skip((queryParams.PageNumber - 1)
                    * queryParams.PageSize)
                .Take(queryParams.PageSize);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = product.Name;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
            existing.Category = product.Category;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
