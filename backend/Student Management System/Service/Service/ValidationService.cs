using Repository.Model;
using Service.DTO;
using Service.Interface;

namespace Service.Service
{
    public class ValidationService : IValidationService
    {
        #region Validate Teacher
        public async Task<ValidationDTO> ValidateTeacher(TeacherDTO teacherDTO)
        {
            var validationRules = new Dictionary<Func<TeacherDTO, bool>, string>
        {
            { dto => !string.IsNullOrWhiteSpace(dto.Name), "Name is required." },
            { dto => !string.IsNullOrWhiteSpace(dto.Email) && IsValidEmail(dto.Email), "Valid Email is required." },
            { dto => dto.Salary > 0, "Salary is required and must be greater than zero." },
            { dto => dto.DateOfBirth != default, "Date of Birth is required." },
            { dto => dto.DateOfEnrollment != default, "Date of Enrollment is required." },
            { dto=> dto.DateOfEnrollment<DateTime.Today, "Date of Enrollment is less than today." },
            { dto=> dto.DateOfBirth<DateTime.Today, "Date of Birth should be less than today's date"},
            { dto => Enum.IsDefined(typeof(ClassLevels), dto.Class), "Invalid class level." },
            { dto => dto.SubjectId > 0, "Valid SubjectId is required." },
            { dto => !string.IsNullOrWhiteSpace(dto.Qualification), "Qualification is required." },
           };

            return await ValidateDTO(teacherDTO, validationRules);
        }
        #endregion



        #region Validate Edit Teacher
        public async Task<ValidationDTO> ValidateEditTeacher(EditTeacherDTO editTeacherDTO)
        {
            var validationRules = new Dictionary<Func<EditTeacherDTO, bool>, string>
        {
            { dto => !string.IsNullOrWhiteSpace(dto.Name), "Name is required." },
            { dto => dto.Salary > 0, "Salary is required and must be greater than zero." },
            { dto => dto.DateOfBirth != default, "Date of Birth is required." },
            { dto => dto.DateOfEnrollment != default, "Date of Enrollment is required." },
            { dto=> dto.DateOfBirth<DateTime.Today, "Date of Birth should be less than today's date"},
            { dto => !string.IsNullOrWhiteSpace(dto.Qualification), "Qualification is required." },
           };

            return await ValidateDTO(editTeacherDTO, validationRules);
        }
        #endregion

        #region Validate Student
        public async Task<ValidationDTO> ValidateStudent(StudentDTO studentDTO)
        {
            var validationRules = new Dictionary<Func<StudentDTO, bool>, string>
        {
            { dto => !string.IsNullOrWhiteSpace(dto.Student.Name), "Name is required." },
            { dto => !string.IsNullOrWhiteSpace(dto.Student.Email) && IsValidEmail(dto.Student.Email), "Valid Email is required." },
            { dto => dto.Student.DateOfBirth != default, "Date of Birth is required." },
            { dto => dto.Student.DateOfEnrollment <DateTime.Today, "Date of Enrollment is required." },
            { dto => dto.Student.RollNumber > 0, "Roll Number must be greater than zero." },
            { dto => dto.Student.RollNumber < 100, "Roll Number must be less than zero." },
            { dto => Enum.IsDefined(typeof(ClassLevels), dto.Class), "Invalid class level." }
        };

            return await ValidateDTO(studentDTO, validationRules);
        }
        #endregion

        #region Validate Student
        public async Task<ValidationDTO> ValidateAddStudentByTeacher(StudentDTO studentDTO)
        {
            var validationRules = new Dictionary<Func<StudentDTO, bool>, string>
        {
            { dto => !string.IsNullOrWhiteSpace(dto.Student.Name), "Name is required." },
            { dto => !string.IsNullOrWhiteSpace(dto.Student.Email) && IsValidEmail(dto.Student.Email), "Valid Email is required." },
            { dto => dto.Student.DateOfBirth != default, "Date of Birth is required." },
            { dto => dto.Student.DateOfEnrollment != default, "Date of Enrollment is required." },
            { dto => dto.Student.DateOfEnrollment != default, "Date of Enrollment is required." },
            { dto => dto.Student.RollNumber > 0, "Roll Number must be greater than zero." },
            //{ dto => dto.RollNumber < 100, "Roll Number must be less than zero." },
            //{ dto => Enum.IsDefined(typeof(ClassLevels), dto.Class), "Invalid class level." }
        };

            return await ValidateDTO(studentDTO, validationRules);
        }
        #endregion

        #region Validate Edit Student
        public async Task<ValidationDTO> ValidateEditStudent(EditStudentDTO editStudentDTO)
        {
            var validationRules = new Dictionary<Func<EditStudentDTO, bool>, string>
        {
            { dto => !string.IsNullOrWhiteSpace(dto.Name), "Name is required." },
            { dto => dto.DateOfBirth != default, "Date of Birth is required." },
            { dto => dto.DateOfEnrollment != default, "Date of Enrollment is required." },
            { dto => dto.RollNumber > 0, "Roll Number must be greater than zero." },

        };

            return await ValidateDTO(editStudentDTO, validationRules);
        }
        #endregion

        #region Generic Validation
        private async Task<ValidationDTO> ValidateDTO<T>(T dto, Dictionary<Func<T, bool>, string> validationRules)
        {
            ValidationDTO response = new ValidationDTO();
            response.Status = 200;
            response.Message = "Validation successful.";

            List<string> errors = new List<string>();

            foreach (var rule in validationRules)
            {
                if (!rule.Key(dto))
                {
                    errors.Add($"Validation failed: {rule.Value}");
                }
            }

            if (errors.Any())
            {
                response.Status = 400;
                response.Message = "Validation failed.";
                response.Errors = errors;
            }

            return response;
        }
        #endregion
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}
