using Service.Interface;
using System.Security.Cryptography;
using System.Text;

namespace Service.Service
{
    public class PasswordHashService : IPasswordHashService
    {
        public string GeneratePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                return hashString;
            }
        }
    }
}
