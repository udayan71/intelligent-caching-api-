using LeaveManagementSystem.DAL;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repository
{
    public class LeaveRepository
    {
        private readonly AppDbContext _db;

        public LeaveRepository(AppDbContext db)
        {
            _db = db;
        }

        // Get All Records
        public List<LeaveRequest> GetAll()
        {
            return _db.LeaveRequests.ToList();
        }

        // Get Single Record By Id
        public LeaveRequest GetById(int id)
        {
            return _db.LeaveRequests.Find(id);
        }

        // Add Record
        public void Add(LeaveRequest leave)
        {
            _db.LeaveRequests.Add(leave);
            _db.SaveChanges();
        }

        // Update Record
        public void Update(LeaveRequest leave)
        {
            _db.Entry(leave).State = EntityState.Modified;
            _db.SaveChanges();
        }

        // Delete Record
        public void Delete(int id)
        {
            var data = _db.LeaveRequests.Find(id);

            if (data != null)
            {
                _db.LeaveRequests.Remove(data);
                _db.SaveChanges();
            }
        }
    }
}