using Repository.Model;

namespace Repository.Interface
{
    public interface IAuthRepo
    {
        Task<Users> Login(string email, string password);
        Task<bool> ResetPassword(int userId, string password);
        Task<bool> CheckIsPasswordReset(int userId);
    }
}
