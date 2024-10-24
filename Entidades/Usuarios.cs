using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace WebApplication1.Entidades
{
    public class Usuarios
    {
        [Key]
        public required string Rut { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public required string Telefono { get; set; }
        public required string Password { get; set; }
        public  bool Admin { get; set; } = false;
        public  DateTime Fecha_registro { get; set; } = DateTime.Now;


    }

}