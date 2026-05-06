using StudentCrudApp.Models;
using StudentCrudApp.Repositories;

namespace StudentCrudApp.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;

        public StudentService(IStudentRepository repo)
        {
            _repo = repo;
        }

        // ✅ GET ALL
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _repo.GetAllAsync();
        }

        // ✅ GET BY ID
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        // ✅ CREATE
        public async Task<Student> CreateStudentAsync(Student student)
        {
            // 👉 You can add business logic here later
            return await _repo.CreateStudent(student);
        }

        // ✅ UPDATE
        public async Task<Student> UpdateStudentAsync(Student student)
        {
            return await _repo.Update(student);
        }

        // ✅ DELETE
        public async Task<bool> DeleteStudentAsync(int id)
        {
            return await _repo.Delete(id);
        }
    }
}