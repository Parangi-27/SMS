using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using Repository.Model;

namespace Repository.Repository
{
    public class AuthRepo : IAuthRepo
    {
        #region props
        public readonly AppDbContext _context;
        #endregion

        #region ctor
        public AuthRepo(AppDbContext context)
        {
            _context = context;
        }
        #endregion

        #region login

        public async Task<Users> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email) && u.Password.Equals(password));
            return user;
        }
        #endregion

        public async Task<bool> ResetPassword(int userId, string password)
        {

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Password = password;
                user.IsPasswordReset = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> CheckIsPasswordReset(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (user.IsPasswordReset == false)
                    return true;
            }
            return false;
        }


    }
}
