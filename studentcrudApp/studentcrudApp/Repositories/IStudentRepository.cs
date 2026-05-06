using StudentCrudApp.Models;

namespace StudentCrudApp.Repositories
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);

        Task<Student> CreateStudent(Student student);

        Task<Student> Update(Student student);

        Task<bool> Delete(int id);
    }
}