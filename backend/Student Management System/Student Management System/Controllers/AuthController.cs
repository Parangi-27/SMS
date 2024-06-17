using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace Student_Management_System.Controllers
{
    public class AuthController : BaseController
    {
        #region props

        private readonly IAuthService _authService;

        #endregion

        #region ctor
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var response = await _authService.Login(loginDTO);
            return StatusCode(response.Status, response);
        }
        [Authorize(Roles = "Teacher")]
        //[Authorize(Roles = "Student")]
        [HttpPost("Reset Password")]
        public async Task<IActionResult> ResetPassword([FromBody] string password)
        {
            var response = await _authService.ResetPassword(password);
            return StatusCode(response.Status, response);
        }
    }
}
