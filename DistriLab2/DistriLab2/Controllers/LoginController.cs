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
                if (user != null)
                {
                    User perAux = _context.Users.Where(p => p.EmailUser == user.EmailUser).FirstOrDefault();
                    DateTime expirationDate = DateTime.UtcNow.AddMinutes(30);
                    string token = generateToken(emailUser, expirationDate);
                    return Ok(new { token = token, user = perAux});

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
                            string passaux = ToSHA256(credential.HashUser);
                            Credential cred = new Credential()
                            {
                                CodCredential = "2",
                                EmailUser = credential.EmailUser,
                                HashUser = passaux
                            };
                            _context.Credentials.Add(cred);
                            await _context.SaveChangesAsync();

                            return Created($"/User/{cred.EmailUser}", cred);
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
        }
}