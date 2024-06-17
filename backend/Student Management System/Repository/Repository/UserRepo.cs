using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class UserRepo : IUserRepo
    {
        #region props
        public readonly AppDbContext _context;
        #endregion

        #region ctor
        public UserRepo(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Add User
        public async Task<Users> AddUser(Users user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;

            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Unique Email
        public async Task<bool> CheckUniqueEmail(string email)
        {
            bool isUnique = await _context.Users.AnyAsync(u => u.Email == email);
            return isUnique;
        }
        #endregion

        #region get students by userId
        public async Task<List<Student>> GetStudentsByUserIds(List<int> userIds)
        {
            return await _context.Students
                                 .Include(s => s.User)
                                 .Where(s => userIds.Contains(s.UserId))
                                 .ToListAsync();
        }
        #endregion

        #region get teachers by userId
        public async Task<List<Teacher>> GetTeachersByUserIds(List<int> userIds)
        {
            return await _context.Teachers
                                 .Include(t => t.User)
                                 .Where(t => userIds.Contains(t.UserId))
                                 .ToListAsync();
        }
        #endregion

        #region get teacher by id
        public async Task<Teacher> GetTeacherById(int userId)
        {
            var teacher = await _context.Teachers.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher != null)
            {
                return teacher;
            }
            return null;
        }
        #endregion

        #region get student by id
        public async Task<Student> getStudentById(int userId)
        {
            var student = await _context.Students.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId);
            if (student != null)
            {
                return student;
            }
            return null;
        }
        #endregion

        #region get email by studentId
        public async Task<string> GetEmailByStudentId(int studentId)
        {
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            return student?.User?.Email;
        }
        #endregion

        #region get email by studentId
        public async Task<Student> GetClassByStudentId(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            return student;
        }
        #endregion

        #region Get all teachers by class
        public async Task<List<Teacher>> GetAllTeachersByClass(ClassLevels classLevel)
        {
            try
            {
                var activeTeachers = await _context.Teachers
                                                .Include(t => t.User)
                                                .Where(t => t.User.IsActive && t.Class == classLevel)
                                                .ToListAsync();

                return activeTeachers;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your application's error handling strategy
                Console.WriteLine($"Error in GetAllTeachersByClass: {ex.Message}");
                return new List<Teacher>(); // Return an empty list to indicate no teachers found or handle error gracefully
            }
        }
        #endregion


    }
}

