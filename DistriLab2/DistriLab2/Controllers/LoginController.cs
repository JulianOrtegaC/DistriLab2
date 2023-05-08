using Azure.Core;
using DistriLab2.Models.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DistriLab2.Controllers
{
        [ApiController]
        [Route("[controller]")]
        public class LoginController : ControllerBase
        {

            private readonly dblab2Context _context;
            private readonly IConfiguration _confi;

            public LoginController(dblab2Context context, IConfiguration confi)
            {
                _context = context;
                _confi = confi;
            }

            [HttpPost]
            public IActionResult Login([FromQuery] string emailUser, [FromQuery] string hash)
            {
                string auxPassword = ToSHA256(hash);
                Credential user = _context.Credentials.Where(c => c.EmailUser == emailUser && c.HashUser == auxPassword).FirstOrDefault();
                User user1 = _context.Users.Where(c => c.EmailUser == emailUser).FirstOrDefault();
                if (user != null)
                {
                    if (user1.StatusUser == "A")
                    {
                        User perAux = _context.Users.Where(p => p.EmailUser == user.EmailUser).FirstOrDefault();
                        DateTime expirationDate = DateTime.UtcNow.AddMinutes(30);
                        string token = generateToken(emailUser, expirationDate);
                        return Ok(new { token = token, user = perAux });
                    }
                    else
                    {
                    return BadRequest(new { message = "El usuario está inactivo" });
                }
                }
                else
                {
                    return BadRequest(new { message = "Credenciales incorrectas" });
                }
            }


            public static string ToSHA256(string s)
            {
                using var sha256 = SHA256.Create();
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(Credential credential)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Credential crede = _context.Credentials.Where(p => p.EmailUser == credential.EmailUser).FirstOrDefault();
            if (crede != null)
            {
                string passaux = ToSHA256(credential.HashUser);
                Credential cred = new Credential()
                {
                    CodCredential = (incrementId(_context) + 1) + "A",
                    EmailUser = credential.EmailUser,
                    HashUser = passaux
                };
                _context.Credentials.Add(cred);
                await _context.SaveChangesAsync();

                return Created($"/User/{cred.EmailUser}", cred);
            }
            return BadRequest(new { message = "La direción de correo eletronico no existe"});
        }


            [HttpGet]
            [Route("Tokenn")]
            public string generateToken(string username, DateTime expirationDate)
            {
#pragma warning disable CS8604 // Posible argumento de referencia nulo
                var keyByte = Encoding.ASCII.GetBytes(_confi.GetSection("settings").GetSection("secretKey").ToString());
#pragma warning restore CS8604 // Posible argumento de referencia nulo
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

            [HttpGet]
            public static int incrementId(dblab2Context dbContext)
            {
                int count = dbContext.Credentials.Count();
                return count;
            }
    }
}