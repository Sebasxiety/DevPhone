using Microsoft.EntityFrameworkCore;

namespace DevPhone.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MCliente> Clientes => Set<MCliente>();
        public DbSet<MUsuario> Usuarios => Set<MUsuario>();
        public DbSet<MDispositivo> Dispositivos => Set<MDispositivo>();
        public DbSet<MOrdenServicio> OrdenesServicio => Set<MOrdenServicio>();
        public DbSet<MRepuesto> Repuestos => Set<MRepuesto>();
        public DbSet<MDetalleRepuesto> DetalleRepuestos => Set<MDetalleRepuesto>();
    }
}
