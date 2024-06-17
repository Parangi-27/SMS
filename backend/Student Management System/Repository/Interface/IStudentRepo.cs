using Repository.Model;

namespace Repository.Interface
{
    public interface IStudentRepo
    {
        Task<List<Attendance>> ViewAttendance(int studentId);
        Task<List<Grades>> ViewGrades(int studentId,int subjectId);
    }
}
