using Repository.Interface;
using Service.DTO;
using Service.Interface;

namespace Service.Service
{
    public class StudentService : IStudentService
    {
        #region props
        private readonly IUserRepo _userRepo;
        private readonly IStudentRepo _studentRepo;
        private readonly IUserService _userService;


        #endregion

        #region ctor
        public StudentService(IUserRepo userRepo, IStudentRepo studentRepo, IUserService userService)
        {
            _userRepo = userRepo;
            _studentRepo = studentRepo;
            _userService = userService;
        }

        #endregion

        #region View Attendance
        public async Task<ResponseDTO> ViewAttendance()
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUserId();
                var student = _userRepo.getStudentById(loggedInUser);

                var attendances = await _studentRepo.ViewAttendance(student.Id);
                if (attendances == null || attendances.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = 404, // Not Found
                        Message = "No attendance records found for the specified student.",
                        Data = null
                    };
                }

                // Transform Attendances to StudentAttendanceDTOs
                var attendanceDTOs = attendances.Select(a => new StudentAttendanceDTO
                {
                    IsPresent = a.isPresent,
                    SubjectId = a.SubjectId,
                    Date = a.Date
                }).ToList();

                return new ResponseDTO
                {
                    Status = 200, // OK
                    Message = "Attendance records retrieved successfully.",
                    Data = attendanceDTOs
                };
            }
            catch (Exception ex)
            {
                // Log the exception somewhere
                return new ResponseDTO
                {
                    Status = 500, // Internal Server Error
                    Message = $"Error retrieving attendance records: {ex.Message}",
                    Data = null
                };
            }
        }
        #endregion 
        
        #region View Grades
        public async Task<ResponseDTO> ViewGrades(int subjectId)
        {
            try
            {
                var loggedInUser = _userService.GetLoggedInUserId();
                var student = _userRepo.getStudentById(loggedInUser);

                var grades = await _studentRepo.ViewGrades(student.Id, subjectId);
                if (grades == null || grades.Count == 0)
                {
                    return new ResponseDTO
                    {
                        Status = 404, // Not Found
                        Message = "No grades records found for the specified student.",
                        Data = null
                    };
                }

                var gradesDTOs = grades.Select(a => new GradesDTO
                {
                    GradeDetails = new CommonGradeDTO
                    {
                        MarksObtained = a.Marks,
                        TotalMarks = a.TotalMarks,
                        Date = a.ExamMonth,
                        Percentage = a.TotalMarks != 0 ? ((double)a.Marks / a.TotalMarks) * 100 : 0
                    },
                }).ToList();

                return new ResponseDTO
                {
                    Status = 200, // OK
                    Message = "Attendance records retrieved successfully.",
                    Data = gradesDTOs
                };
            }
            catch (Exception ex)
            {
                // Log the exception somewhere
                return new ResponseDTO
                {
                    Status = 500, // Internal Server Error
                    Message = $"Error retrieving attendance records: {ex.Message}",
                    Data = null
                };
            }
        }
        #endregion
    }

}
