using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Models
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<MCliente> Clientes { get; set; }
        public DbSet<MUsuario> Usuarios { get; set; }
        public DbSet<MDispositivo> Dispositivos { get; set; }
        public DbSet<MOrdenServicio> OrdenesServicio { get; set; }
        public DbSet<MRepuesto> Repuestos { get; set; }
        public DbSet<MDetalleRepuesto> DetallesRepuesto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rompemos la cascada entre Dispositivo y OrdenServicio
            modelBuilder.Entity<MOrdenServicio>()
                .HasOne(o => o.Dispositivo)
                .WithMany(d => d.Ordenes)
                .HasForeignKey(o => o.IdDispositivo)
                .OnDelete(DeleteBehavior.Restrict);

            // Rompemos la cascada entre Cliente y OrdenServicio
            modelBuilder.Entity<MOrdenServicio>()
                .HasOne(o => o.Cliente)
                .WithMany(c => c.Ordenes)
                .HasForeignKey(o => o.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            // Rompemos también la cascada entre Usuario y OrdenServicio
            modelBuilder.Entity<MOrdenServicio>()
                .HasOne(o => o.Usuario)
                .WithMany(u => u.Ordenes)
                .HasForeignKey(o => o.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}
