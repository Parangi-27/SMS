using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Service.Service
{
    public class JWTTokenService : IJWTTokenService
    {
        #region props
        private readonly IConfiguration _config;
        #endregion

        #region ctor
        public JWTTokenService(IConfiguration configuration)
        {
            _config = configuration;
        }
        #endregion

        #region generate JWT token
        public string GenerateJwtToken(string userRole, string userId)
        {
            var uId = userId.ToString();
            List<Claim> claims = new List<Claim>
            {
                new Claim("Id", uId),
                new Claim(ClaimTypes.Role, userRole),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("JWT:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(_config["JWT:Issuer"], _config["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: cred
            );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        #endregion
    }
}
