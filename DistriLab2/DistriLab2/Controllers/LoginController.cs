using Azure.Core;
using DistriLab2.Models.DB;
using DistriLab2.utils;
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
            public IActionResult Login([FromQuery] string emailUser, [FromQuery] string password)
            {
                string auxPassword = ToSHA256(password);
                var resultado = (from c1 in _context.Credentials
                             join c2 in _context.Users on c1.EmailUser equals c2.EmailUser
                             select new
                             {
                                 EmailUser = c1.EmailUser,
                                 StatusCode = c2.StatusUser,
                                 HashUser = c1.HashUser
                             }
                             ).Where(c => c.EmailUser == emailUser && c.HashUser == auxPassword).FirstOrDefault();
                if (resultado != null)
                {
                    if (resultado.StatusCode == "A")
                    {
                        DateTime expirationDate = DateTime.UtcNow.AddMinutes(30);
                        string token = Tokens.generateToken(_confi, emailUser, expirationDate);
                        
                        return Ok(new { token = token, user = emailUser });
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
            public async Task<IActionResult> Register(string nameuser, string email, string password)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                User auxNewUser = new User
                {
                    NameUser=nameuser,
                    EmailUser=email,
                    StatusUser="A"
                };
                _context.Users.Add(auxNewUser);
                User crede = _context.Users.Where(p => p.EmailUser == email).FirstOrDefault();
         
                if (crede == null)
                {
                    string passaux = ToSHA256(password);
                    Credential cred = new Credential()
                    {
                        CodCredential = (Tokens.incrementId(_context) + 1) + "A",
                        EmailUser = email,
                        HashUser = passaux
                    };
                    _context.Credentials.Add(cred);
                    await _context.SaveChangesAsync();

                    return Created($"/User/{cred.EmailUser}", cred);
                }
                return BadRequest(new { message = "El usuario ya existe"});
            }
    }
}