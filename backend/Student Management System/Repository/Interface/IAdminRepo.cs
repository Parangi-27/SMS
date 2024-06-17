using Repository.Model;

namespace Repository.Interface
{
    public interface IAdminRepo
    {
        Task<Teacher> AddTeacher(Teacher teacher);
        Task<Student> AddStudent(Student student);
        Task<bool> TeacherExistsWithSubject(ClassLevels teacherClass, int subjectId);
        Task<List<Teacher>> GetAllTeachers();
        Task<Teacher> GetTeacherById(int userId);
        Task<List<Student>> GetAllStudents();
        Task<Student> GetStudentById(int userId);
        Task<bool> EditTeacher(Teacher editedTeacher);
        Task<bool> DeleteTeacher(Teacher toBeDeleteTeacher);
        Task<bool> EditStudent(Student editedStudents);
        Task<bool> DeleteStudent(Student toBeDeleteStudent);
        Task<bool> DeleteUserAndTeacher(Teacher teacher);
        Task<bool> DeleteUserAndStudent(Student student);
        Task<List<Attendance>> GetAllAttendances();
        Task<Attendance> GetAttendanceById(int id);
        Task<bool> EditAttendance(Attendance attendance);
        Task<List<Grades>> GetAllStudentGrades();
        Task<bool> EditGrades(Grades grades);
        Task<Grades> FindGradesById(int id);
    }
}
