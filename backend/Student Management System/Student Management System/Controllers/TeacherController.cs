using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;
using Service.Service;

namespace Student_Management_System.Controllers
{
    [Authorize(Roles ="Teacher")]
    public class TeacherController : BaseController
    {
        #region props

        private readonly ITeacherService _teacherService;

        #endregion

        #region ctor
        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }
        #endregion

        #region Add Student API
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] StudentDTO studentDTO)
        {
            var response = await _teacherService.AddStudent(studentDTO);
            return StatusCode(response.Status, response);
        }
        #endregion
        
        #region Get Today's Attendance API
        [HttpGet("GetTodayAttendance")]
        public async Task<IActionResult> GetTodayAttendance()
        {
            var response = await _teacherService.GetTodayAttendance();
            return StatusCode(response.Status, response);
        }
        #endregion
        
        #region Record Attendance API
        [HttpPost("RecordAttendance")]
        public async Task<IActionResult> RecordAttendance([FromBody] List<AttendanceDTO> attendanceDTOs)
        {
            var response = await _teacherService.RecordAttendance(attendanceDTOs);
            return StatusCode(response.Status, response);
        }
        #endregion 
        
        #region Attendance History API
        [HttpGet("AttendanceHistory")]
        public async Task<IActionResult> AttendanceHistory()
        {
            var response = await _teacherService.GetAttendanceHistory();
            return StatusCode(response.Status, response);
        }
        #endregion
        
        #region Attendance History By Month API
        [HttpGet("AttendanceHistory/{month}")]
        public async Task<IActionResult> AttendanceHistoryByMonth(int month)
        {
            var response = await _teacherService.GetAttendanceHistoryByMonth(month);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Record Grades
        [HttpPost("RecordGrades")]
        public async Task<IActionResult> RecordGrades([FromBody] GradesDTO gradeDTO)
        {
            var response = await _teacherService.RecordGrades(gradeDTO);
            return StatusCode(response.Status, response);
        }
        #endregion 

        #region View Grades History
        [HttpGet("ViewAllGradeHistory")]
        public async Task<IActionResult> ViewAllGradeHistory()
        {
            var response = await _teacherService.ViewGradeHistory();
            return StatusCode(response.Status, response);
        }
        #endregion

        #region View Grades History For Student
        [HttpGet("ViewStudentGradeHistory/{studentId}")]
        public async Task<IActionResult> ViewStudentGradeHistory(int studentId)
        {
            var response = await _teacherService.ViewStudentDetails(studentId);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Edit Attendance 
        [HttpPost("EditAttendanceByTeacher")]
        public async Task<IActionResult> EditAttendanceByTeacher(int attendanceId, bool isPresent)
        {
            var response = await _teacherService.EditAttendance(attendanceId, isPresent);
            return StatusCode(response.Status, response);
        }
        #endregion 

        #region Edit Grades API
        [HttpPost("EditGradesByTeacher")]
        public async Task<IActionResult> EditGradesByTeacher(int id, [FromBody] GradesDTO gradesDTO)
        {
            var response = await _teacherService.EditGrades(id, gradesDTO);
            return StatusCode(response.Status, response);
        }
        #endregion
    }
}
