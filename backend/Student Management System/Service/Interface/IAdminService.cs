using Service.DTO;

namespace Service.Interface
{
    public interface IAdminService
    {
        Task<ResponseDTO> AddTeacher(TeacherDTO teacherDTO);
        Task<ResponseDTO> AddStudent(StudentDTO studentDTO);
        Task<ResponseDTO> GetTeachers();
        Task<ResponseDTO> GetTeacherById(int id);
        Task<ResponseDTO> GetStudents();
        Task<ResponseDTO> GetStudentById(int id);
        Task<ResponseDTO> EditStudent(int id, EditStudentDTO editStudentDTO);
        Task<ResponseDTO> EditTeacher(int id, EditTeacherDTO editTeacherDTO);
        Task<ResponseDTO> DeleteStudent(int studentId);
        Task<ResponseDTO> DeleteTeacher(int teacherId);
        Task<ResponseDTO> ViewAttendance();
        Task<ResponseDTO> EditAttendance(int attendanceId, bool isPresent);
        Task<ResponseDTO> ViewGrades();
        Task<ResponseDTO> EditGrades(int id, GradesDTO gradesDTO);
    }
}
