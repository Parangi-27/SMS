using Service.DTO;

namespace Service.Interface
{
    public interface IStudentService
    {
        Task<ResponseDTO> ViewAttendance();
        Task<ResponseDTO> ViewGrades(int subjectId);
    }
}
