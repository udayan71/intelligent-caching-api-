using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Category)
                      .HasMaxLength(50);
            });
        }
    }
}