using Repository.Model;

namespace Repository.Interface
{
    public interface IUserRepo
    {
        Task<Users> AddUser(Users user);
        Task<bool> CheckUniqueEmail(string email);
        Task<List<Student>> GetStudentsByUserIds(List<int> userIds);
        Task<List<Teacher>> GetTeachersByUserIds(List<int> userIds);
        Task<Teacher> GetTeacherById(int userId);
        Task<Student> getStudentById(int userId);
        Task<string> GetEmailByStudentId(int studentId);
        Task<Student> GetClassByStudentId(int studentId);
        Task<List<Teacher>> GetAllTeachersByClass(ClassLevels classLevel);

    }
}
