namespace WebApplication1.Entidades
{
    public class Tickets
    {
        public int Id { get; set; }
        public required string Asunto { get; set; }
        public required string Descripcion { get; set; }
        public string Estado { get; set; } = "Abierto";
        public DateTime Fecha_creacion { get; set; } = DateTime.Now;
        public required string Usuario_rut { get; set; }

        public Usuarios Usuario { get; set; }

        public ICollection<Comentarios> Comentarios { get; set; }
    }
}