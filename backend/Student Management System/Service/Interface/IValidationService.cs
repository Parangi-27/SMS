using Service.DTO;

namespace Service.Interface
{
    public interface IValidationService
    {
        Task<ValidationDTO> ValidateTeacher(TeacherDTO teacherDTO);
        Task<ValidationDTO> ValidateStudent(StudentDTO studentDTO);
        Task<ValidationDTO> ValidateAddStudentByTeacher(StudentDTO studentDTO);
        Task<ValidationDTO> ValidateEditTeacher(EditTeacherDTO editTeacherDTO);
        Task<ValidationDTO> ValidateEditStudent(EditStudentDTO editStudentDTO);

    }
}
