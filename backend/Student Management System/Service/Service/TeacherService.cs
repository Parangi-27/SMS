
using AutoMapper;
using Repository.Interface;
using Repository.Model;
using Service.Constant;
using Service.DTO;
using Service.Interface;

namespace Service.Service
{
    public class TeacherService : ITeacherService
    {
        #region props
        private readonly ITeacherRepo _teacherRepo;
        private readonly IAdminRepo _adminRepo;
        private readonly IUserRepo _userRepo;
        private readonly IValidationService _validationService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IPasswordHashService _passwordHash;

        #endregion

        #region ctor
        public TeacherService(ITeacherRepo teacherRepo, IAdminRepo adminRepo, IUserRepo userRepo, IUserService userService, IValidationService validationService, IPasswordHashService passwordHash, IMapper mapper, IEmailService emailService)
        {
            _teacherRepo = teacherRepo;
            _adminRepo = adminRepo;
            _userRepo = userRepo;
            _passwordHash = passwordHash;
            _validationService = validationService;
            _mapper = mapper;
            _userService = userService;
            _emailService = emailService;
        }

        #endregion

        #region Add Student
        public async Task<ResponseDTO> AddStudent(StudentDTO studentDTO)
        {
            try
            {
                var validationResult = await _validationService.ValidateAddStudentByTeacher(studentDTO);

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
                var addUser = await _userRepo.AddUser(user);
                if (addUser == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.UserAddingFailure
                    };
                }

                var loggedInUser = _userService.GetLoggedInUserId();

                var getTeacherById = await _userRepo.GetTeacherById(loggedInUser);
                var student = _mapper.Map<Student>(studentDTO);
                student.UserId = user.Id;
                student.Class = getTeacherById.Class;
                student.CreatedOn = DateTime.UtcNow;
                student.CreatedBy = loggedInUser;

                // Adding in student table
                var addStudent = await _teacherRepo.AddStudent(student);
                if (addStudent == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = ResponseMessage.UserAddingFailure
                    };
                }
                // Prepare email body
                var emailBody = string.Format(ResponseMessage.RegisterEmailBodyTemplate, user.Role, RoleTypes.Teacher, password);
                var emailDTO = new EmailDTO
                {
                    Email = new List<string> { studentDTO.Student.Email },
                    Subject = ResponseMessage.RegisterEmailSubject,
                    Body = emailBody
                };
                bool isEmailSent = await _emailService.SendEmail(emailDTO);

                var teachersEmail = await _teacherRepo.GetTeachersEmailsByClass(getTeacherById.Class, getTeacherById.Id);
                var emailTeacherBody = string.Format(ResponseMessage.NewStudentToTeacherBody, student.User.Email, RoleTypes.Teacher);
                var emailTeacherDTO = new EmailDTO
                {
                    Email = teachersEmail.ToList(),
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

        #region Get Attendance
        public async Task<ResponseDTO> GetTodayAttendance()
        {
            //it will show list of attendances and then using this we'll record attendance
            try
            {
                var loggedInUser = _userService.GetLoggedInUserId();

                var teacherById = await _adminRepo.GetTeacherById(loggedInUser);
                if (teacherById == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = ResponseMessage.SomethingWentWrong
                    };
                }

                var students = await _teacherRepo.GetStudentsByClass(teacherById.Class);
                var attendanceRecords = students.Select(s => new AttendanceDTO
                {
                    StudentId = s.Id,
                    IsPresent = false,
                    //TeacherId = teacherById.Id,
                    SubjectId = teacherById.SubjectId,
                    //ClassLevels = teacherById.Class
                }).ToList();

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = attendanceRecords,
                    Message = "Attendance records created successfully"
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

        #region Record Attendance
        public async Task<ResponseDTO> RecordAttendance(List<AttendanceDTO> attendanceDTOs)
        {
            try
            {
                // Get the logged-in user
                var loggedInUser = _userService.GetLoggedInUserId();
                var getTeacherById = await _userRepo.GetTeacherById(loggedInUser);
                if (getTeacherById == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = "Logged-in teacher not found."
                    };
                }

                var currentDate = DateTime.Now.Date;

                // Check if attendance is already recorded for today for the logged-in teacher
                bool isAlreadyRecorded = await _teacherRepo.IsAttendanceAlreadyRecorded(getTeacherById.Id, currentDate);
                if (isAlreadyRecorded)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.BadRequest,
                        Message = "Attendance for today is already recorded."
                    };
                }

                // Map AttendanceDTO to Attendance using AutoMapper
                var attendanceRecords = _mapper.Map<List<Attendance>>(attendanceDTOs);

                foreach (var record in attendanceRecords)
                {
                    record.TeacherId = getTeacherById.Id;
                    record.ClassLevel = getTeacherById.Class;
                    record.Date = currentDate;
                    record.CreatedOn = currentDate;
                    record.CreatedBy = loggedInUser;
                }

                // Save attendance records
                var isSaved = await _teacherRepo.RecordAttendance(attendanceRecords);

                if (!isSaved)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = "Failed to save attendance records"
                    };
                }

                // Check for absences and send emails to other teachers in the same class
                var absentStudents = attendanceRecords.Where(a => !a.isPresent).ToList();
                if (absentStudents.Any())
                {
                    var otherTeachers = await _teacherRepo.GetTeachersEmailsByClass(getTeacherById.Class, getTeacherById.Id);
                    var ccEmails = string.Join(",", otherTeachers);
                    foreach (var student in absentStudents)
                    {
                        var studentEmail = await _userRepo.GetEmailByStudentId(student.StudentId); // Assuming studentEmail is a string
                        var emailBody = string.Format(ResponseMessage.AbsentEmailBodyTemplate, studentEmail, getTeacherById.User.Name);

                        var emailDTO = new EmailDTO
                        {
                            Email = new List<string> { studentEmail }, // Convert single email string to a list
                            CC = otherTeachers.ToList(), // Assuming otherTeachers is IEnumerable<string> or List<string>
                            Subject = "Student Absence Notification",
                            Body = emailBody
                        };


                        // Send absent email
                        var emailSent = await _emailService.SendAbsentEmail(emailDTO);

                        if (!emailSent)
                        {
                            // Log email sending failure or handle as needed
                            // For simplicity, assuming email sending failure does not affect attendance recording
                        }
                    }
                }

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = "Attendance records saved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}"
                };
            }
        }


        #endregion

        #region Attendance History
        public async Task<ResponseDTO> GetAttendanceHistory()
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUserId();
                var teacherById = await _adminRepo.GetTeacherById(loggedInUser);

                var attendanceRecords = await _teacherRepo.GetAttendanceHistoryByTeacherId(teacherById.Id);
                if (attendanceRecords == null || !attendanceRecords.Any())
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.Ok,
                        Message = ResponseMessage.NoRecords,
                    };
                }
                var attendanceHistoryDTOs = attendanceRecords.Select(a => new AttendanceHistoryDTO
                {
                    StudentName = a.Student.User.Name,
                    IsPresent = a.isPresent,
                    Date = a.Date
                }).ToList();

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = attendanceHistoryDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}"
                };
            }

        }
        #endregion

        #region Attendance History By Month
        public async Task<ResponseDTO> GetAttendanceHistoryByMonth(int month)
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUserId();
                var teacherById = await _adminRepo.GetTeacherById(loggedInUser);

                // Calculate start and end dates for the month
                var year = DateTime.UtcNow.Year;
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var attendanceRecords = await _teacherRepo.GetAttendanceByDateRange(teacherById.Id, startDate, endDate);
                if (attendanceRecords == null || !attendanceRecords.Any())
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.Ok,
                        Message = ResponseMessage.NoRecords,
                    };
                }
                var attendanceHistoryDTOs = attendanceRecords.Select(a => new AttendanceHistoryDTO
                {
                    StudentName = a.Student.User.Name,
                    IsPresent = a.isPresent,
                    Date = a.Date
                }).ToList();

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = attendanceHistoryDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        #endregion

        #region Record Grades
        public async Task<ResponseDTO> RecordGrades(GradesDTO gradesDTO)
        {
            try
            {
                // Get the logged-in user
                var loggedInUser = _userService.GetLoggedInUserId();
                var teacherById = await _adminRepo.GetTeacherById(loggedInUser);


                // Map GradesDTO to Grades using AutoMapper
                var grade = _mapper.Map<Grades>(gradesDTO);
                var studentClass = await _userRepo.GetClassByStudentId(gradesDTO.StudentId);

                if (teacherById.Class != studentClass.Class)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.Forbidden,
                        Message = "You are not authorized to add grades for this student."
                    };
                }

                //check for already added marks remaining

                grade.TeacherId = teacherById.Id;
                grade.CreatedOn = DateTime.UtcNow.Date;
                grade.CreatedBy = loggedInUser;

                // Assuming you have a _gradeRepo to handle database operations for Grades
                var isSaved = await _teacherRepo.RecordGrades(grade);

                if (!isSaved)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.InternalServerError,
                        Message = "Failed to record grade."
                    };
                }
                var emailBody = string.Format(ResponseMessage.GradeEmailBodyTemplate, teacherById.Subject, teacherById.User.Name);
                var emailDTO = new EmailDTO
                {
                    Email = new List<string> { studentClass.User.Email },
                    Subject = "Grade is Added",
                    Body = emailBody,
                };
                _emailService.SendEmail(emailDTO);
                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Message = "Grade recorded successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
        #endregion

        #region View Grades
        public async Task<ResponseDTO> ViewGradeHistory()
        {
            try
            {
                var loggedInUserId = _userService.GetLoggedInUserId();
                var teacher = await _userRepo.GetTeacherById(loggedInUserId);

                if (teacher == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = "Teacher not found."
                    };
                }

                var grades = await _teacherRepo.GetGradeHistoryByTeacherId(teacher.Id);

                if (grades == null)
                {
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.NoGradeRecordsFound,
                    };
                }
                var gradeHistoryDTOs = grades.Select(g => new GradeHistoryDTO
                {
                    StudentId = g.Student.Id,
                    StudentName = g.Student.User.Name,
                    SubjectId = g.Teacher.SubjectId,
                    GradeDetails = new CommonGradeDTO
                    {
                        MarksObtained = g.Marks,
                        TotalMarks = g.TotalMarks,
                        Date = g.ExamMonth,
                        Percentage = g.TotalMarks != 0 ? ((double)g.Marks / g.TotalMarks) * 100 : 0,
                    },
                }).ToList();

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = gradeHistoryDTOs
                };
            }
            catch (Exception ex)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.InternalServerError,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        #endregion

        #region Get Student Details
        public async Task<ResponseDTO> ViewStudentDetails(int studentId)
        {
            var student = await _userRepo.GetEmailByStudentId(studentId);
            var loggedInUser = _userService.GetLoggedInUserId();
            var teacherById = await _adminRepo.GetTeacherById(loggedInUser);

            if (student == null)
            {
                return new ResponseDTO { Status = StatusCode.InternalServerError, Message = ResponseMessage.SomethingWentWrong };
            }
            var grades = await _teacherRepo.GetGradeHistoryByStudentId(studentId, teacherById.Id);
            if (grades == null || !grades.Any())
            {
                return new ResponseDTO
                {
                    Status = StatusCode.NotFound,
                    Message = "No grades found for the specified student."
                };
            }
            var studentGradesDTOs = grades.Select(g => new StudentDetailsWithGradesDTO
            {
                StudentDetails = new StudentDTO
                {
                    Student = new CommonStudentDTO
                    {
                        Name = g.Student.User.Name,
                        Email = g.Student.User.Email,
                        DateOfBirth = g.Student.User.DateOfBirth,
                        DateOfEnrollment = g.Student.User.DateOfEnrollment,
                        RollNumber = g.Student.RollNumber,
                    },
                    Class = g.Student.Class
                },
                GradeDetails = new CommonGradeDTO
                {
                    MarksObtained = g.Marks,
                    TotalMarks = g.TotalMarks,
                    Date = g.ExamMonth,
                },
                Percentage = g.TotalMarks != 0 ? ((double)g.Marks / g.TotalMarks) * 100 : 0,

            }).ToList();
            if (studentGradesDTOs == null)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.NotFound,
                    Message = "No grades found for the specified student."
                };
            }
            return new ResponseDTO
            {
                Status = StatusCode.Ok,
                Data = studentGradesDTOs,
            };
        }
        #endregion

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

    }
}

