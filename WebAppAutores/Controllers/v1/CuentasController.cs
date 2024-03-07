using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebAppAutores.DTOs;
using WebAppAutores.Servicios;

namespace WebAppAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider, // for encryption
            HashService hashService
        )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector("valor_unico_y_secreto");
        }

        [HttpPost("registrar", Name = "registrar-usuario")] // api/cuentas/registrar
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
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login", Name = "login-usuario")]
        public async Task<ActionResult<RespuestaAutenticationDTO>> Login(CredencialesUsuarioDTO credencialesUsuario)
        {
            var result = await signInManager.PasswordSignInAsync(
                    credencialesUsuario.Email,
                    credencialesUsuario.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

            if (result.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }

        }

        [HttpGet("renovar-token", Name = "renovar-token")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticationDTO>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var credencialesUsuario = new CredencialesUsuarioDTO()
            {
                Email = email,
            };

            return await ConstruirToken(credencialesUsuario);
        }

        // takes a registered user and make it admin by adding the esAdmin claim
        [HttpPost("hacer-admin", Name = "hacer-admin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);

            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }

        // takes a registered admin and revoke its privileges by removing the esAdmin claim
        [HttpPost("remover-admin", Name = "remover-admin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDTO editarAdminDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDTO.Email);

            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));

            return NoContent();
        }

        private async Task<RespuestaAutenticationDTO> ConstruirToken(CredencialesUsuarioDTO credencialesUsuario)
        {
            // information in claim is public, don't send sensitive data
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email); // find the used in the DB
            var claimsDB = await userManager.GetClaimsAsync(usuario); // get its claims

            claims.AddRange(claimsDB); // additional claims are concatenated

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
