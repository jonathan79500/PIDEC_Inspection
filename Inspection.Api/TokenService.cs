using Inspection.Domain.Entities.User.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inspection.Api
{
    public class TokenService
    {
        private readonly UserManager<SystematicUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly static JwtConfig _jwtConfig;

        public TokenService(UserManager<SystematicUser> userManager, IConfiguration configuration,JwtConfig jwtConfig)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtConfig = jwtConfig;
        }

        public async Task<string> GenerateToken(SystematicUser user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateJWTToken(SystematicUser user) { 
        
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new [] { 
                    new Claim("Id",user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub , user.Email),
                    new Claim(JwtRegisteredClaimNames.Email , user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
                }),
                 Expires = DateTime.UtcNow.AddHours(4),
                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha512)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return jwtToken;
        }
    }

}
