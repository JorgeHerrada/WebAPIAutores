using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAppAutores.DTOs;

namespace WebAppAutores.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController: ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager, 
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost("registrar")] // api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticationDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuario)
        {
            // create Identity user with received information
            var user = new IdentityUser
            {
                UserName = credencialesUsuario.Email,
                Email = credencialesUsuario.Email
            };
            
            var result = await userManager.CreateAsync(user, credencialesUsuario.Password);

            if (result.Succeeded)
            {
                return ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        private RespuestaAutenticationDTO ConstruirToken(CredencialesUsuarioDTO credencialesUsuario)
        {
            // information in claim is public, don't send sensitive data
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["keyjwt"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1); // should be just minutes in case of a reall app

            var securityToken = new JwtSecurityToken(
                issuer: null, 
                audience: null, 
                claims: claims, 
                expires: expiration, 
                signingCredentials: creds
            );

            return new RespuestaAutenticationDTO()
            {
                // cast it to a string that can be returned
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiration
            };
        }
    }
}
