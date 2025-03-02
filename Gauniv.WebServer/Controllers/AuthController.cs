using Gauniv.WebServer.Dtos;
using Gauniv.WebServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Controllers
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AuthController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Vérifie si l'utilisateur existe en utilisant l'email
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized(); // Si les identifiants sont incorrects, retourne Unauthorized
            }

            // Vérification du rôle de l'utilisateur (admin ou utilisateur classique)
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return Unauthorized(); // Si l'utilisateur n'est pas un admin, retourne Unauthorized
            }

            // Crée le JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ma-super-cle-secrete-de-32-caracteres!!!");           

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName), // Inclut le nom d'utilisateur dans les revendications du token
                    new Claim(ClaimTypes.NameIdentifier, user.Id), // Inclut l'ID de l'utilisateur dans les revendications du token
                    new Claim(ClaimTypes.Role, "Admin") 
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Durée de validité du token (ici 1 heure)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) // Utilise la clé secrète pour signer le token
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token); // Convertit le token en chaîne de caractères

            // Retourne le token à l'utilisateur
            return Ok(new { Token = tokenString });
        }


    }
}
