using Service.DTO;

namespace Service.Interface
{
    public interface ITeacherService
    {
        Task<ResponseDTO> AddStudent(StudentDTO studentDTO);
        Task<ResponseDTO> GetTodayAttendance();
        Task<ResponseDTO> RecordAttendance(List<AttendanceDTO> attendanceDTOs);
        Task<ResponseDTO> GetAttendanceHistory();
        Task<ResponseDTO> GetAttendanceHistoryByMonth(int month);
        Task<ResponseDTO> RecordGrades(GradesDTO gradesDTO);
        Task<ResponseDTO> ViewGradeHistory();
        Task<ResponseDTO> ViewStudentDetails(int studentId);
        Task<ResponseDTO> EditAttendance(int attendanceId, bool isPresent);
        Task<ResponseDTO> EditGrades(int id, GradesDTO gradesDTO);
    }
}