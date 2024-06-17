using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class StudentRepo : IStudentRepo
    {
        #region props
        public readonly AppDbContext _context;

        #endregion

        #region ctor
        public StudentRepo(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region View Attendance
        public async Task<List<Attendance>> ViewAttendance(int studentId)
        {
            return await _context.Attendances
                                 .Where(t => t.StudentId == studentId)
                                 .ToListAsync();
        }
        #endregion
        
        #region View Grades
        public async Task<List<Grades>> ViewGrades(int studentId, int subjectId)
        {
            return await _context.Grades
                         .Include(g => g.Student)  
                         .ThenInclude(s => s.User)  
                         .Include(g => g.Teacher)  
                         .ThenInclude(t => t.Subject)  
                         .Where(t => t.StudentId == studentId && t.Teacher.SubjectId == subjectId)
                         .ToListAsync();
        }
        #endregion
    }
}
