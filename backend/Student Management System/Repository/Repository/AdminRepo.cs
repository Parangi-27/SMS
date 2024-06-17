using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class AdminRepo : IAdminRepo
    {
        #region props
        public readonly AppDbContext _context;

        #endregion

        #region ctor
        public AdminRepo(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region add teacher
        public async Task<Teacher> AddTeacher(Teacher teacher)
        {
            try
            {
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return teacher;
            }
            catch (Exception)
            {
                return null;
            }
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

        #region teacher exits with particular subject
        public async Task<bool> TeacherExistsWithSubject(ClassLevels teacherClass, int subjectId)
        {
            bool isTeacherExists = await _context.Teachers.AnyAsync(u => u.Class == teacherClass && u.SubjectId == subjectId);
            return isTeacherExists;
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

        #region get all teachers
        public async Task<List<Teacher>> GetAllTeachers()
        {
            var activeTeachers = await _context.Teachers
             .Include(s => s.User)
             .Where(s => s.User.IsActive)
             .ToListAsync();

            return activeTeachers ?? new List<Teacher>();
        }
        #endregion

        #region student by id
        public async Task<Student> GetStudentById(int userId)
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

        #region get all students
        public async Task<List<Student>> GetAllStudents()
        {
            var activeStudents = await _context.Students
            .Include(s => s.User)
            .Where(s => s.User.IsActive)
            .ToListAsync();

            return activeStudents ?? new List<Student>();
        }
        #endregion

        #region edit teacher
        public async Task<bool> EditTeacher(Teacher editedTeacher)
        {
            try
            {
                _context.Teachers.Update(editedTeacher);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region delete teacher
        public async Task<bool> DeleteTeacher(Teacher toBeDeleteTeacher)
        {
            try
            {
                _context.Teachers.Update(toBeDeleteTeacher);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region edit student
        public async Task<bool> EditStudent(Student editedStudent)
        {
            try
            {
                _context.Students.Update(editedStudent);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region delete student
        public async Task<bool> DeleteStudent(Student toBeDeleteStudent)
        {
            try
            {
                _context.Students.Update(toBeDeleteStudent);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region delete user and teacher
        public async Task<bool> DeleteUserAndTeacher(Teacher teacher)
        {
            try
            {
                _context.Teachers.Remove(teacher);
                _context.Users.Remove(teacher.User);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region delete user and student
        public async Task<bool> DeleteUserAndStudent(Student student)
        {
            try
            {
                _context.Students.Remove(student);
                _context.Users.Remove(student.User);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region get all attendances
        public async Task<List<Attendance>> GetAllAttendances()
        {
            var attendance = await _context.Attendances
                                           .Include(a => a.Student)
                                           .ThenInclude(s => s.User) // If Student has a User property
                                           .Include(a => a.Teacher)
                                           .ThenInclude(t => t.Subject) // If Teacher has a Subject property
                                           .OrderBy(a => a.Date)
                                           .ThenBy(a => a.ClassLevel)
                                           .ToListAsync();

            if (attendance == null)
            {
                return new List<Attendance>();
            }

            return attendance;
        }

        #endregion

        #region get attendances by id
        public async Task<Attendance> GetAttendanceById(int id)
        {
            var attendance = await _context.Attendances
                                           .FirstOrDefaultAsync(a => a.Id == id);
            return attendance;
        }
        #endregion

        #region edit attendances by id
        public async Task<bool> EditAttendance(Attendance attendance)
        {
            try
            {
                _context.Attendances.Update(attendance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        public async Task<List<Grades>> GetAllStudentGrades()
        {
            var grades = await _context.Grades
                                       .Include(g => g.Student)
                                       .ThenInclude(s => s.User)
                                       .Include(g => g.Teacher)
                                       .ThenInclude(t => t.Subject)
                                       .OrderBy(g => g.ExamMonth)
                                       .ToListAsync();

            if (grades == null)
            {
                return new List<Grades>();
            }

            return grades;
        }

        public async Task<Grades> FindGradesById(int id)
        {
            var grade = await _context.Grades
                             .Include(g => g.Teacher)
                             .FirstOrDefaultAsync(t => t.Id == id);
            if (grade == null)
            {
                return null;
            }
            return grade;
        }

        public async Task<bool> EditGrades(Grades grades)
        {
            try
            {
                _context.Update(grades);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
