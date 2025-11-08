
using Inspection.Application.Common.Interfaces.Authentication;
using Inspection.Application.Contracts.Persistence;
using Inspection.Application.User;
using Inspection.Domain.Entities.User.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inspection.Infrastructure.Authentication;
public class JwtTokenGenerator : IJwtTokenGenerator
{

    string IJwtTokenGenerator.GenerateToken(SystematicUser currentUser,List<string> userRoles)
    {
        
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e");

       

        var claims = new List<Claim> {  
                    new Claim("Id",currentUser.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub , currentUser.UserName!),
                    new Claim(JwtRegisteredClaimNames.Email , currentUser.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())};

        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
           
        Expires = DateTime.UtcNow.AddHours(4),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
        };
        
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }

    
}
