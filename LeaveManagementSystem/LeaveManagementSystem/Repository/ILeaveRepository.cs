using System.Collections.Generic;
using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Repository
{
    public interface ILeaveRepository
    {
        List<LeaveRequest> GetAll();
        LeaveRequest GetById(int id);
        void Add(LeaveRequest leave);
        void Update(LeaveRequest leave);
        void Delete(int id);
    }
}
