using System.Net.Sockets;

namespace WebApplication1.Entidades
{
    public class Comentarios
    {
        public int Id { get; set; }
        public required string Comentario { get; set; }
        public DateTime Fecha_comentario { get; set; } = DateTime.Now;
        public required string Usuario_rut { get; set; }
        public required int Ticket_id { get; set; }

        public Tickets Tickets { get; set; } 
        public Usuarios Usuarios { get; set; }

    }
}
