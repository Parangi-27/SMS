using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class TeacherRepo : ITeacherRepo
    {
        #region props
        public readonly AppDbContext _context;
        #endregion

        #region ctor
        public TeacherRepo(AppDbContext context)
        {
            _context = context;
        }

        #endregion

        #region class teachers
        public Task<List<string>> ClassTeachers(ClassLevels classLevel)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region fetch class teachers
        public async Task<List<string>> ClassTeachersEmails(ClassLevels classLevel)
        {
            var teacherEmails = await _context.Teachers
                .Where(t => t.Class == classLevel)
                .Select(t => t.User.Email)
                .ToListAsync();
            return teacherEmails;
        }
        #endregion

        #region attendance
        public async Task<bool> RecordAttendance(List<Attendance> attendanceRecords)
        {
            try
            {
                await _context.Attendances.AddRangeAsync(attendanceRecords);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                // Log the exception (e.g., using a logging framework)
                return false;
            }
        }
        #endregion

        #region get students by class
        public async Task<List<Student>> GetStudentsByClass(ClassLevels classLevel)
        {
            var students = await _context.Students
           .Include(s => s.User)
           .Where(s => s.Class == classLevel && s.User.IsActive)
           .ToListAsync();

            return students;
        }
        #endregion

        #region add student
        public async Task<Student> AddStudent(Student student)
        {
            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return student;

            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region check if already recorded attendance
        public async Task<bool> IsAttendanceAlreadyRecorded(int teacherId, DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.Attendances
                .AsNoTracking()
                .AnyAsync(a => a.TeacherId == teacherId && a.Date.Date == dateOnly);
        }
        #endregion

        #region get teachers emails by class
        public async Task<List<string>> GetTeachersEmailsByClass(ClassLevels classLevel, int excludeTeacherId)
        {
            var teachers = await _context.Teachers
                .AsNoTracking()
                .Where(t => t.Class == classLevel && t.Id != excludeTeacherId)
                .Select(t => t.User.Email)
                .ToListAsync();

            return teachers;
        }
        #endregion

        #region get teachers emails by class
        public async Task<List<Attendance>> GetAttendanceHistoryByTeacherId(int teacherId)
        {
            var attendanceHistory = await _context.Attendances
           .Include(a => a.Student)
           .ThenInclude(s => s.User)
           .Where(a => a.TeacherId == teacherId)
           .ToListAsync();

            return attendanceHistory;
        }
        #endregion  

        #region get attendance history by month
        public async Task<List<Attendance>> GetAttendanceByDateRange(int teacherId, DateTime startDate, DateTime endDate)
        {
            var attendanceHistory = await _context.Attendances
                .Include(a => a.Student)
                .ThenInclude(s => s.User)
                .Where(a => a.TeacherId == teacherId && a.Date >= startDate && a.Date <= endDate)
                .ToListAsync();

            return attendanceHistory;
        }
        #endregion

        #region record grades
        public async Task<bool> RecordGrades(Grades grade)
        {
            try
            {
                await _context.Grades.AddAsync(grade);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
        #endregion

        #region view grade history for teacher
        public async Task<List<Grades>> GetGradeHistoryByTeacherId(int teacherId)
        {
            return await _context.Grades
                 .Include(g => g.Student)  
                 .ThenInclude(s => s.User)  
                 .Include(g => g.Teacher)  
                 .Where(g => g.TeacherId == teacherId).ToListAsync();
        
        }
        #endregion

        #region view grade history for student
        public async Task<List<Grades>> GetGradeHistoryByStudentId(int studentId, int teacherId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .ThenInclude(s => s.User)
                .Include(g => g.Teacher)
                .Where(g => g.TeacherId == teacherId && g.StudentId == studentId)
                .ToListAsync();
        }
        #endregion

    }
}
