using System;
using DistriLab2.Models.DB;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DistriLab2.utils
{
	public static class Tokens
	{

        public static string generateToken(IConfiguration _confi,string username, DateTime expirationDate)
        {
            var keyByte = Encoding.ASCII.GetBytes(_confi.GetSection("settings").GetSection("secretKey").ToString());
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = expirationDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyByte), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(tokenConfig);
            return token;
        }

        public static int incrementId(dblab2Context dbContext)
        {
            int count = dbContext.Credentials.Count();
            return count;
        }
    }
}

