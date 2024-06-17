using Service.DTO;

namespace Service.Interface
{
    public interface IAuthService
    {
        Task<ResponseDTO> Login(LoginDTO loginDTO);
        Task<ResponseDTO> ResetPassword(string password);
    }
}
