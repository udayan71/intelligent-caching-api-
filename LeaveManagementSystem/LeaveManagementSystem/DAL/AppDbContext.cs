using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace LeaveManagementSystem.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<LeaveRequest> LeaveRequests { get; set; }
    }
}
