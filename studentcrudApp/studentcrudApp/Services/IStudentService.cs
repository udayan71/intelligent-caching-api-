using StudentCrudApp.Models;

namespace StudentCrudApp.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllStudentsAsync();

        Task<Student?> GetStudentByIdAsync(int id);

        Task<Student> CreateStudentAsync(Student student);

        Task<Student> UpdateStudentAsync(Student student);

        Task<bool> DeleteStudentAsync(int id);
    }
}