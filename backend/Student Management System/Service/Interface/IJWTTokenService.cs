
namespace Service.Interface
{
    public interface IJWTTokenService
    {
        string GenerateJwtToken(string userId, string userRole);
    }
}
