using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace Student_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        #region props

        private readonly IAdminService _adminService;

        #endregion

        #region ctor
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        #endregion

        #region Add Teacher API
        [HttpPost("AddTeacher")]
        public async Task<IActionResult> AddTeacher([FromBody] TeacherDTO teacherDTO)
        {
            var response = await _adminService.AddTeacher(teacherDTO);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Add Student API
        [HttpPost("AddStudent")]
        public async Task<IActionResult> AddStudent([FromBody] StudentDTO studentDTO)
        {
            var response = await _adminService.AddStudent(studentDTO);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get TeacherAPI
        [HttpGet("GetTeacher")]
        public async Task<IActionResult> GetTeachers()
        {
            var response = await _adminService.GetTeachers();
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get TeacherAPI By Id
        [HttpPost("GetTeacherById")]
        public async Task<IActionResult> GetTeacherById(int id)
        {
            var response = await _adminService.GetTeacherById(id);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get StudentAPI
        [HttpGet("GetStudent")]
        public async Task<IActionResult> GetStudents()
        {
            var response = await _adminService.GetStudents();
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get Student By Id
        [HttpPost("GetStudentById")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var response = await _adminService.GetStudentById(id);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Edit Student API
        [HttpPost("EditStudent/{id}")]
        public async Task<IActionResult> EditStudent(int id, [FromBody] EditStudentDTO editedStudentDTO)
        {
            var response = await _adminService.EditStudent(id, editedStudentDTO);
            return StatusCode(response.Status, response);
        }
        #endregion 
        
        #region Edit Teacher API
        [HttpPost("EditTeacher/{id}")]
        public async Task<IActionResult> EditTeacher(int id, [FromBody] EditTeacherDTO editedTeacherDTO)
        {
            var response = await _adminService.EditTeacher(id, editedTeacherDTO);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Delete Teacher
        [HttpDelete("DeleteTeacher/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var response = await _adminService.DeleteTeacher(id);
            return StatusCode(response.Status, response);
        }
        #endregion
         
        #region Delete Student
        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var response = await _adminService.DeleteStudent(id);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get All Students Grades
        [HttpGet("GetAllGrades")]
        public async Task<IActionResult> GetAllGrades()
        {
            var response = await _adminService.ViewGrades();
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Edit Grades API
        [HttpPost("EditGrades")]
        public async Task<IActionResult> EditGrades(int id, [FromBody] GradesDTO gradesDTO)
        {
            var response = await _adminService.EditGrades(id, gradesDTO);
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Get All Students Attendance
        [HttpGet("GetAllAttendance")]
        public async Task<IActionResult> GetAllAttendance()
        {
            var response = await _adminService.ViewAttendance();
            return StatusCode(response.Status, response);
        }
        #endregion

        #region Edit Attendance 
        [HttpPost("EditAttendance")]
        public async Task<IActionResult> EditAttendance(int attendanceId, bool isPresent)
        {
            var response = await _adminService.EditAttendance(attendanceId, isPresent);
            return StatusCode(response.Status, response);
        }
        #endregion 

    }
}
