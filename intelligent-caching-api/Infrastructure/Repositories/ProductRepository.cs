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

        public async Task<IEnumerable<Product>> GetAllAsync(int pageNumber,
    int pageSize)
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
