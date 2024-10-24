using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Text.Json;
using WebApplication1.Entidades;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApplication1.Services;
using System.Text;

namespace WebApplication1.Controllers
{
    [Route("/api/v1/usuarios/")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly JwtService jwtService;

        public UsersController(ApplicationDbContext context, JwtService jwtService)
        {
            this.context = context;
            this.jwtService = jwtService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> Get()
        {
            var usuario = await context.Usuarios.ToListAsync();

            if (usuario == null || !usuario.Any())
            {
                return NotFound(new { message = "No se encontraron usuarios", code = 404 });
            }

            return Ok(new { usuario, message = "usuarios encontrados con éxito", code = 200 });
        }


        [HttpPost]
        public async Task<ActionResult<Usuarios>> Post([FromBody] Usuarios usuarios)
        {
           
            try
            {
                usuarios.Password = BCrypt.Net.BCrypt.HashPassword(usuarios.Password);
                context.Add(usuarios);
                await context.SaveChangesAsync();
                return Ok(new { data = usuarios, message = "Usuario Registrado con éxito", code = 201 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Ocurrió un error al crear el Comentario", error = ex.InnerException?.Message ?? ex.Message, code = 500, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuarios>> Post([FromBody] Login usuarios)
        {

            try
            {
                var user = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuarios.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(usuarios.Password, user.Password))
                {
                    return Unauthorized(new { message = "Email o contraseña incorrecta" });
                }

                var usuario = new
                {
                    user.Rut,
                    user.Nombre,
                    user.Apellido,
                    user.Email,
                    user.Telefono,
                    user.Admin
                };

                var token = jwtService.GenerateToken(user.Email);
                return Ok(new { usuario, token, message = "Usuario logueado con éxito", code = 200 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "No se pudo loguear al usuario", error = ex.InnerException?.Message ?? ex.Message, code = 500, stackTrace = ex.StackTrace });
            }
        }


    }
}
