using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using WebApplication1.Dtos;
using WebApplication1.Entidades;
using WebApplication1.Requests;

namespace WebApplication1.Controllers
{

    [Route("/api/v1/tickets/")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TicketsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tickets>>> Get()
        {
            var tickets = await context.Tickets
                .Include(t => t.Usuario) // Incluye la entidad Usuario
                .ToListAsync();

            var ticketResponses = tickets.Select(ticket => new
            {
                ticket.Id,
                ticket.Asunto,
                ticket.Descripcion,
                ticket.Estado,
                ticket.Fecha_creacion,
                ticket.Usuario_rut,
                Usuario = new
                {
                    Nombre = ticket.Usuario.Nombre,
                    Apellido = ticket.Usuario.Apellido,
                }
            });

            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = "No se encontraron tickets", code = 404 });
            }

            return Ok(new { data = ticketResponses, message = "Tickets encontrados con éxito", code = 200 });
        }

        [HttpGet("rut/{rut}")]

        public async Task<ActionResult<IEnumerable<Tickets>>> Get(string rut)
        {
            var tickets = await context.Tickets
                .Include(t => t.Usuario) // Incluye la entidad Usuario
                .Where(t => t.Usuario_rut == rut)
                .ToListAsync();

            var ticketResponses = tickets.Select(ticket => new
            {
                ticket.Id,
                ticket.Asunto,
                ticket.Descripcion,
                ticket.Estado,
                ticket.Fecha_creacion,
                ticket.Usuario_rut,
                Usuario = new
                {
                    Nombre = ticket.Usuario.Nombre,
                    Apellido = ticket.Usuario.Apellido,
                }
            });

            if (tickets == null || !tickets.Any())
            {
                return NotFound(new { message = "No se encontraron tickets", code = 404 });
            }

            return Ok(new { data = ticketResponses, message = "Tickets encontrados con éxito", code = 200 });
        }

        [HttpGet("{id:int}")]
        public IActionResult GetTicket(int id)

        {
            Console.Write("Hola");
            var ticket = context.Tickets
                .Include(t => t.Usuario)
                .Include(t => t.Comentarios)
                .ThenInclude(c => c.Usuarios)
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            var ticketResponse = new
            {
                ticket.Id,
                ticket.Asunto,
                ticket.Descripcion,
                ticket.Estado,
                ticket.Fecha_creacion,
                ticket.Usuario_rut,
                Usuarios = new
                {
                    ticket.Usuario.Nombre,
                    ticket.Usuario.Apellido,
                    ticket.Usuario.Email,
                    ticket.Usuario.Telefono,
                    ticket.Usuario.Rut
                },
                Comentarios = ticket.Comentarios.Select(c => new
                {
                    c.Id,
                    c.Comentario,
                    c.Fecha_comentario,
                    c.Usuario_rut,
                    c.Ticket_id,
                    Usuarios = new
                    {
                        c.Usuarios.Nombre,
                        c.Usuarios.Apellido,
                        c.Usuarios.Email,
                        c.Usuarios.Telefono,
                        c.Usuarios.Rut
                    }
                }).ToList()
            };

            return Ok(new { data = ticketResponse, message = "Ticket encontrado con éxito", code = 200 });
        }


        [HttpPost]
        public async Task<ActionResult<Tickets>> Post([FromBody] TicketCreateDto ticketDto)
        {
            try
            {
                var ticket = new Tickets
                {
                    Asunto = ticketDto.Asunto, 
                    Descripcion = ticketDto.Descripcion, 
                    Usuario_rut = ticketDto.Rut,
                 
                };
                context.Add(ticket);
                
                await context.SaveChangesAsync();
                return Ok(new { data = ticket, message = "Ticket creado con éxito", code = 201 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Ocurrió un error al crear el ticket", error = ex.Message, code = 500, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("comentary")]
        public async Task<ActionResult<Comentarios>> PostComentario([FromBody] ComentariosCreateDto comentariosDto)
        {
            try
            {
                var comentarios = new Comentarios
                {
                    Ticket_id = comentariosDto.Ticket,
                    Comentario = comentariosDto.Comentario,
                    Usuario_rut = comentariosDto.Rut,

                };
                context.Add(comentarios);
                await context.SaveChangesAsync();
                return Ok(new { data = comentarios, message = "Comentario creado con éxito", code = 201 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Ocurrió un error al crear el Comentario", error = ex.Message, code = 500, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] UpdateRequest request)

        {
            try
            {
                var ticket = await context.Tickets.FindAsync(id);

                if (ticket == null)
                {
                    return NotFound(new { message = "Ticket no encontrado", code = 404 });
                }

                ticket.Estado = request.Estado;
                await context.SaveChangesAsync();
                return Ok(new { data = ticket, message = "Estado cambiado con éxito", code = 201 });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Ocurrió un error al crear el Comentario", error = ex.InnerException?.Message ?? ex.Message, code = 500, stackTrace = ex.StackTrace });
            }
        }

     
    }
}
