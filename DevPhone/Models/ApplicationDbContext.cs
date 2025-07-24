using Microsoft.EntityFrameworkCore;

namespace DevPhone.Models
{
    public class ApplicationDbContext : DbContext
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
    }
}
