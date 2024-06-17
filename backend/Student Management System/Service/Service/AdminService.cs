using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repository.Interface;
using Repository.Model;
using Service.Constant;
using Service.DTO;
using Service.Interface;

namespace Service.Service
{
    public class AdminService : IAdminService
    {
        #region props
        private readonly IAdminRepo _adminRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;
        private readonly IPasswordHashService _passwordHash;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        #endregion

        #region ctor
        public AdminService(IAdminRepo adminRepo, IUserRepo userRepo, IUserService userService, IHttpContextAccessor httpContextAccessor, IValidationService validationService, IPasswordHashService passwordHash, IMapper mapper, IEmailService emailService)
        {
            _adminRepo = adminRepo;
            _userRepo = userRepo;
            _userService = userService;
            _passwordHash = passwordHash;
            _validationService = validationService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;

        }

        #endregion

        #region Add Teacher
        public async Task<ResponseDTO> AddTeacher(TeacherDTO teacherDTO)
        {
            try
            {
                // Validate teacher data
                var validationResult = await _validationService.ValidateTeacher(teacherDTO);

                if (!validationResult.IsValid)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.ValidationError,
                        Error = string.Join("; ", validationResult.Errors)
                    };
                }

                // Check if email is unique
                var isExists = await _userRepo.CheckUniqueEmail(teacherDTO.Email);
                if (isExists)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.EmailAlreadyExists
                    };
                }

                // Check if the teacher already exists with the same class and subject
                bool isAlreadyTeacher = await _adminRepo.TeacherExistsWithSubject(teacherDTO.Class, teacherDTO.SubjectId);
                if (isAlreadyTeacher)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.TeacherAlreadyExists
                    };
                }

                // Generate password
                var formattedDateOfBirth = teacherDTO.DateOfBirth.ToString("yyyyMMdd");
                var cleanedName = teacherDTO.Name.Replace(" ", "");
                var password = cleanedName + formattedDateOfBirth;
                string passHash = _passwordHash.GeneratePasswordHash(password);

                // Map teacher DTO to user model and set properties
                var user = _mapper.Map<Users>(teacherDTO);
                user.Role = RoleTypes.Teacher;
                user.Password = passHash;
                user.IsActive = true;
                user.IsPasswordReset = false;
                //adding in User table
                var newUser = await _userRepo.AddUser(user);
                if (newUser == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.UserAddingFailure
                    };
                }


                // Map teacher DTO to teacher model and set properties
                var teacher = _mapper.Map<Teacher>(teacherDTO);
                teacher.UserId = user.Id;
                teacher.CreatedOn = DateTime.UtcNow;

                //adding in teacher table
                var addTeacher = await _adminRepo.AddTeacher(teacher);
                if (addTeacher == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.UserAddingFailure
                    };
                }

                // Prepare email body
                var emailBody = string.Format(ResponseMessage.RegisterEmailBodyTemplate, user.Role, RoleTypes.Admin, password);
                var emailDTO = new EmailDTO
                {
                    Email = new List<string> {teacherDTO.Email},
                    Subject = ResponseMessage.RegisterEmailSubject,
                    Body = emailBody
                };


                // Send email
                bool isEmailSent = await _emailService.SendEmail(emailDTO);
                if (!isEmailSent)
                {
                    var rollbackUser = await _adminRepo.GetTeacherById(addTeacher.Id);
                    await _adminRepo.DeleteUserAndTeacher(rollbackUser);
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.EmailSendFailure
                    };
                }

                //await _emailLogRepo.addEmailLog()
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserAddingSuccess
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }


        #endregion

        #region Add Student
        public async Task<ResponseDTO> AddStudent(StudentDTO studentDTO)
        {
            try
            {
                var validationResult = await _validationService.ValidateStudent(studentDTO);

                if (!validationResult.IsValid)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.ValidationError,
                        Error = string.Join("; ", validationResult.Errors)
                    };
                }
                var isExists = await _userRepo.CheckUniqueEmail(studentDTO.Student.Email);
                if (isExists)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.EmailAlreadyExists
                    };
                }

                var formattedDateOfBirth = studentDTO.Student.DateOfBirth.ToString("yyyyMMdd");
                var password = studentDTO.Student.Name.Replace(" ", "") + formattedDateOfBirth; // Remove white spaces in name

                string passHash = _passwordHash.GeneratePasswordHash(password);

                var user = _mapper.Map<Users>(studentDTO);
                user.Role = RoleTypes.Student;
                user.Password = passHash;
                user.IsPasswordReset = false;
                user.IsActive = true;

                // Adding in User table
                //error handling remaining
                await _userRepo.AddUser(user);

                var loggedInUser = _userService.GetLoggedInUserId();


                var student = _mapper.Map<Student>(studentDTO);
                student.UserId = user.Id;
                student.CreatedOn = DateTime.UtcNow;
                student.CreatedBy = loggedInUser;

                // Adding in student table
                var addStudent = await _adminRepo.AddStudent(student);
                if (addStudent == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.UserAddingFailure
                    };
                }
                // Prepare email body
                var emailBody = string.Format(ResponseMessage.RegisterEmailBodyTemplate, user.Role, RoleTypes.Admin, password);
                var emailDTO = new EmailDTO
                {
                    Email = new List<string> { studentDTO.Student.Email },
                    Subject = ResponseMessage.RegisterEmailSubject,
                    Body = emailBody
                };
                bool isEmailSent = await _emailService.SendEmail(emailDTO);
                var teachersEmail = await _userRepo.GetAllTeachersByClass(studentDTO.Class);
                var emailTeacherBody = string.Format(ResponseMessage.NewStudentToTeacherBody, student.User.Email, RoleTypes.Admin);
                var emailTeacherDTO = new EmailDTO
                {
                    Email = teachersEmail.Select(t => t.User.Email).ToList(),
                    Subject = ResponseMessage.NewStudentNotiTeacherSubject,
                    Body = emailBody
                };
                bool isEmailSentToTeachers = await _emailService.SendEmail(emailTeacherDTO);
                if (!isEmailSent)
                {
                    var rollbackUser = await _adminRepo.GetStudentById(addStudent.Id);
                    await _adminRepo.DeleteUserAndStudent(rollbackUser);
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.EmailSendFailure
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserAddingSuccess
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }
        #endregion

        #region Get All Teachers
        public async Task<ResponseDTO> GetTeachers()
        {
            try
            {
                var teachers = await _adminRepo.GetAllTeachers();

                // Filter out inactive teachers
                var activeTeachers = teachers.Where(t => t.User.IsActive).ToList();

                if (activeTeachers.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NoContent,
                        Message = ResponseMessage.NoContent
                    };
                }

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = activeTeachers,
                    Message = "Teacher list"
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }
        #endregion

        #region Get Teacher By Id
        public async Task<ResponseDTO> GetTeacherById(int userId)
        {
            try
            {
                var teacher = await _adminRepo.GetTeacherById(userId);
                if (teacher == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = teacher,

                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }
        #endregion

        #region Get All Students
        public async Task<ResponseDTO> GetStudents()
        {
            try
            {
                var students = await _adminRepo.GetAllStudents();
                var studentDetailsList = students.Select(s => new GetStudentDetailsDTO
                {
                    userId = s.UserId,
                    RollNumber = s.RollNumber,
                    Name = s.User.Name,
                    Email = s.User.Email,
                    DateOfBirth = s.User.DateOfBirth,
                    DateOfEnrollment = s.User.DateOfEnrollment,
                    Class = s.Class,
                }).ToList();

                if (studentDetailsList.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NoContent,
                        Message = ResponseMessage.NoContent
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = studentDetailsList,
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }
        #endregion

        #region Get Student By Id
        public async Task<ResponseDTO> GetStudentById(int userId)
        {
            try
            {
                var student = await _adminRepo.GetStudentById(userId);
                if (student == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = student,
                };
            }
            catch (Exception)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = ResponseMessage.SomethingWentWrong
                };
            }
        }
        #endregion

        #region Edit Student
        public async Task<ResponseDTO> EditStudent(int id, EditStudentDTO editStudentDTO)
        {
            try
            {
                var validationResult = await _validationService.ValidateEditStudent(editStudentDTO);
                if (!validationResult.IsValid)
                {
                    return new ResponseDTO { Status = StatusCode.BadRequest, Message = ResponseMessage.ValidationError, Error = string.Join("; ", validationResult.Errors) };
                }
                var isStudent = await _adminRepo.GetStudentById(id);
                if (isStudent == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound,
                    };
                }
                isStudent.RollNumber = editStudentDTO.RollNumber;
                isStudent.User.Name = editStudentDTO.Name;
                isStudent.User.DateOfBirth = editStudentDTO.DateOfBirth;
                isStudent.User.DateOfEnrollment = editStudentDTO.DateOfEnrollment;
                isStudent.ModifiedBy = RoleTypes.Admin;
                isStudent.ModifiedOn = DateTime.UtcNow;


                bool isEdited = await _adminRepo.EditStudent(isStudent);

                if (!isEdited)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.UserEditingFailure,
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserAddingSuccess,
                };


            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}",
                };
            }
        }

        #endregion

        #region Edit Teacher
        public async Task<ResponseDTO> EditTeacher(int id, EditTeacherDTO editTeacherDTO)
        {
            try
            {
                var validationResult = await _validationService.ValidateEditTeacher(editTeacherDTO);
                if (!validationResult.IsValid)
                {
                    return new ResponseDTO { Status = StatusCode.BadRequest, Message = ResponseMessage.ValidationError, Error = string.Join("; ", validationResult.Errors) };
                }
                var isTeacher = await _adminRepo.GetTeacherById(id);
                if (isTeacher == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound,
                    };
                }
                isTeacher.User.Name = editTeacherDTO.Name;
                isTeacher.User.DateOfBirth = editTeacherDTO.DateOfBirth;
                isTeacher.User.DateOfEnrollment = editTeacherDTO.DateOfEnrollment;
                isTeacher.Qualification = editTeacherDTO.Qualification;
                isTeacher.Salary = editTeacherDTO.Salary;
                isTeacher.ModifiedOn = DateTime.UtcNow;


                bool isEdited = await _adminRepo.EditTeacher(isTeacher);

                if (!isEdited)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.UserEditingFailure,
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserAddingSuccess,
                };


            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}",
                };
            }
        }

        #endregion

        #region Delete Teacher 
        public async Task<ResponseDTO> DeleteTeacher(int teacherId)
        {
            try
            {
                var teacher = await _adminRepo.GetTeacherById(teacherId);
                if (teacher == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound
                    };
                }

                // Deactivate the associated user
                teacher.User.IsActive = false;

                bool isDeleted = await _adminRepo.DeleteTeacher(teacher);
                if (!isDeleted)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.SomethingWentWrong
                    };
                }

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserDeleteSuccess
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}",
                };
            }
        }
        #endregion

        #region Delete Student

        public async Task<ResponseDTO> DeleteStudent(int studentId)
        {
            try
            {
                var student = await _adminRepo.GetStudentById(studentId);
                if (student == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound
                    };
                }

                // Deactivate the associated user
                student.User.IsActive = false;

                bool isDeleted = await _adminRepo.DeleteStudent(student);
                if (!isDeleted)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.SomethingWentWrong
                    };
                }

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = ResponseMessage.UserDeleteSuccess
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}",
                };
            }
        }

        #endregion

        public async Task<ResponseDTO> ViewAttendance()
        {
            try
            {
                var attendances = await _adminRepo.GetAllAttendances();
                if (attendances == null || attendances.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = 404, // Not Found
                        Message = "No attendance records found.",
                        Data = null
                    };
                }

                var attendancesDTOs = attendances.Select(a => new AttendanceDTO
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.User.Name,
                    ClassLevel = a.ClassLevel,
                    Date = a.Date,
                    IsPresent = a.isPresent
                }).ToList();

                return new ResponseDTO
                {
                    Status = 200, // OK
                    Message = "Attendance records retrieved successfully.",
                    Data = attendancesDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}",
                };
            }
        }
        public async Task<ResponseDTO> EditAttendance(int attendanceId, bool isPresent)
        {
            try
            {
                var getAttendance = await _adminRepo.GetAttendanceById(attendanceId);
                if (getAttendance == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = "No attendance records found.",
                    };
                }
                getAttendance.isPresent = isPresent;
                getAttendance.ModifiedBy = 1;
                getAttendance.ModifiedOn = DateTime.UtcNow;


                bool isUpdate = await _adminRepo.EditAttendance(getAttendance);
                if (!isUpdate)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.SomethingWentWrong,
                    };
                }
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = "Attendance edited successfully",
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"{ex.Message}",
                };
            }
        }

        public async Task<ResponseDTO> ViewGrades()
        {
            try
            {
                var grades = await _adminRepo.GetAllStudentGrades();
                if (grades == null || grades.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = 404, // Not Found
                        Message = "No grades records found.",
                        Data = null
                    };
                }

                var gradesDTOs = grades.Select(a => new GradesDTO
                {
                    StudentId = a.StudentId,
                    Class = a.Student.Class,
                    SubjectId = a.Teacher.SubjectId,
                    GradeDetails = new CommonGradeDTO
                    {
                        MarksObtained = a.Marks,
                        TotalMarks = a.TotalMarks,
                        Date = a.ExamMonth,
                        Percentage = a.TotalMarks != 0 ? (double)a.Marks / a.TotalMarks * 100 : 0
                    },
                }).ToList();

                return new ResponseDTO
                {
                    Status = 200, // OK
                    Message = "Grades records retrieved successfully.",
                    Data = gradesDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = 500, // Internal Server Error
                    Message = $"Error: {ex.Message}",
                };
            }
        }

        public async Task<ResponseDTO> EditGrades(int id, GradesDTO gradesDTO)
        {
            try
            {
                if (id == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = "Grade ID is required.",
                    };
                }

                // Retrieve the existing grade entry from the database
                var existingGrade = await _adminRepo.FindGradesById(id);
                if (existingGrade == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = "Grade record not found.",
                    };
                }

                // Update the necessary fields
                existingGrade.StudentId = gradesDTO.StudentId;
                existingGrade.Teacher.SubjectId = gradesDTO.SubjectId; // Assuming Teacher has a SubjectId property
                existingGrade.Marks = gradesDTO.GradeDetails.MarksObtained;
                existingGrade.TotalMarks = gradesDTO.GradeDetails.TotalMarks;
                //existingGrade.ExamMonth = gradesDTO.GradeDetails.Date;
                existingGrade.ModifiedOn = DateTime.UtcNow; // Update modification date
                existingGrade.ModifiedBy = 1;// Add the user ID who modified this record (if applicable)

                await _adminRepo.EditGrades(existingGrade);
                // Save the changes to the database
                //_context.Grades.Update(existingGrade);
                //await _context.SaveChangesAsync();

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = "Grade record updated successfully.",
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"{ex.Message}",
                };
            }
        }

        public Task<ResponseDTO> EditAttendance(int attendanceId)
        {
            throw new NotImplementedException();
        }
    }
}
