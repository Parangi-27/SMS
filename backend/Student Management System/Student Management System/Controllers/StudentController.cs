using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Service.Service;

namespace Student_Management_System.Controllers
{
    public class StudentController : Controller
    {
        #region props

        private readonly IStudentService _studentService;

        #endregion

        #region ctor
        public StudentController(IStudentService studentService)
        {
           _studentService=studentService;
        }
        #endregion
        #region Get Attendance
        [HttpGet("StudentAttendance")]
        public async Task<IActionResult> StudentAttendance()
        {
            var response = await _studentService.ViewAttendance();
            return StatusCode(response.Status, response);
        }
        #endregion
        
        #region Get Attendance
        [HttpGet("StudentGrades")]
        public async Task<IActionResult> StudentGrades(int subjectId)
        {
            var response = await _studentService.ViewGrades(subjectId);
            return StatusCode(response.Status, response);
        }
        #endregion
    }
}
