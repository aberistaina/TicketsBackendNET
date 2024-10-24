using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using WebApplication1.Entidades;

namespace WebApplication1
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Tickets> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>()
                .HasKey(u => u.Rut);

            modelBuilder.Entity<Tickets>()
                .HasOne(t => t.Usuario)
                .WithMany() // No es necesario especificar la colección de Tickets aquí
                .HasForeignKey(t => t.Usuario_rut)
                .OnDelete(DeleteBehavior.NoAction); // Cambiar a NoAction

            // Relación entre Comentario y Ticket
            modelBuilder.Entity<Comentarios>()
                .HasOne(c => c.Tickets)
                .WithMany(t => t.Comentarios)
                .HasForeignKey(c => c.Ticket_id)
                .OnDelete(DeleteBehavior.NoAction); // Cambiar a NoAction

            // Relación entre Comentario y Usuario
            modelBuilder.Entity<Comentarios>()
                .HasOne(c => c.Usuarios)
                .WithMany() // No es necesario especificar la colección de Comentarios aquí
                .HasForeignKey(c => c.Usuario_rut)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
