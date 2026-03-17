using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using truyenthongso.Models;

namespace truyenthongso.Common
{
    public static class TokenLogin
    {
        public static string GenerateToken(List<Claim>? claim, Jwt _jwt)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creadentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_jwt.Issuer,
                _jwt.Issuer,
                expires: DateTime.Now.AddMinutes(12000),
                claims: claim,
                signingCredentials: creadentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
