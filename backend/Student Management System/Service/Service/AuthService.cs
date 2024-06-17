using AutoMapper;
using Microsoft.AspNetCore.Http;
using Repository.Interface;
using Service.Constant;
using Service.DTO;
using Service.Interface;

namespace Service.Service
{
    public class AuthService : IAuthService
    {
        #region props
        private readonly IAuthRepo _authRepo;
        private readonly IMapper _mapper;
        private readonly IJWTTokenService _jwtToken;
        private readonly IPasswordHashService _passwordHash;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region ctor
        public AuthService(IAuthRepo authRepo, IMapper mapper, IJWTTokenService jwtToken, IHttpContextAccessor httpContextAccessor, IPasswordHashService passwordHash)
        {
            _authRepo = authRepo;
            _mapper = mapper;
            _jwtToken = jwtToken;
            _passwordHash = passwordHash;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region login
        public async Task<ResponseDTO> Login(LoginDTO loginDTO)
        {
            var passHash = _passwordHash.GeneratePasswordHash(loginDTO.Password);
            var user = await _authRepo.Login(loginDTO.Email, passHash);

            if (user != null)
            {
                if (!user.IsActive)
                {
                    // User is not active
                    return new ResponseDTO
                    {
                        Status = StatusCode.NotFound,
                        Message = ResponseMessage.UserNotFound,
                    };
                }

                // Proceed with login if user is active and password is not reset
                string userRoleString = user.Role.ToString();
                string userId = user.Id.ToString();
                string token = _jwtToken.GenerateJwtToken(userRoleString, userId);

                return new ResponseDTO
                {
                    Status = StatusCode.Ok,
                    Data = new
                    {
                        Token = token,
                        IsResetPassword = user.IsPasswordReset
                    },
                    Message = ResponseMessage.LoginSuccessful,
                };
            }
            else
            {
                // Invalid email or password
                return new ResponseDTO
                {
                    Status = StatusCode.BadRequest,
                    Message = ResponseMessage.InvalidEmailOrPassword
                };
            }
        }
        #endregion

        #region reset password
        public async Task<ResponseDTO> ResetPassword(string password)
        {
            int loggedInUser = 0;
            var loggedInUserClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Id");
            if (loggedInUserClaim != null && int.TryParse(loggedInUserClaim.Value, out int result))
            {
                loggedInUser = result;
            }

            bool isPasswordResetted = await _authRepo.CheckIsPasswordReset(loggedInUser);
            if (!isPasswordResetted)
            {
                return new ResponseDTO
                {
                    Status = StatusCode.BadRequest,
                    Message = ResponseMessage.PasswordResetRequired
                };
            }

            var hashPassword = _passwordHash.GeneratePasswordHash(password);
            bool isResetted = await _authRepo.ResetPassword(loggedInUser, hashPassword);
            if (!isResetted)
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
                Message = ResponseMessage.PasswordResetSuccessful
            };
        }
        #endregion
    }
}
