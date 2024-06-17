using Repository.Model;

namespace Repository.Interface
{
    public interface ITeacherRepo
    {
        Task<List<string>> ClassTeachers(ClassLevels classLevel);
        Task<bool> RecordAttendance(List<Attendance> attendance);
        Task<List<Student>> GetStudentsByClass(ClassLevels classLevel);
        Task<Student> AddStudent(Student student);
        Task<bool> IsAttendanceAlreadyRecorded(int teacherId, DateTime date);
        Task<List<string>> GetTeachersEmailsByClass(ClassLevels classLevel, int excludeTeacherId);
        Task<List<Attendance>> GetAttendanceHistoryByTeacherId(int teacherId);
        Task<List<Attendance>> GetAttendanceByDateRange(int teacherId, DateTime startDate, DateTime endDat);
        Task<bool> RecordGrades(Grades grades);
        Task<List<Grades>> GetGradeHistoryByTeacherId(int teacherId);
        Task<List<Grades>> GetGradeHistoryByStudentId(int studentId, int teacherId);

    }
}
