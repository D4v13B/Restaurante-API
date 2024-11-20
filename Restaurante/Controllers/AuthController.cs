using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurante.Datos;
using Restaurante.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurante.Controllers
{
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            Usuario? usuarioAuth = new Db().Login(model.Email, model.Password);
            if (usuarioAuth == null)
            {
                return Unauthorized(new
                {
                    msg = "Credenciales Invalidas"
                });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, usuarioAuth.UsuarioTipo.Tipo),
                new Claim(ClaimTypes.Email, usuarioAuth.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: _configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { Token = tokenString });
        }
    }
}
