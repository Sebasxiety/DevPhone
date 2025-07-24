using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class DetalleRepuestoService : IDetalleRepuestoService
    {
        private readonly ApplicationDbContext _context;

        public DetalleRepuestoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MDetalleRepuesto>> GetAllAsync() =>
            _context.DetallesRepuesto
                .Include(dr => dr.OrdenService)
                .Include(dr => dr.Repuesto)
                .ToListAsync();

        public Task<MDetalleRepuesto> GetByIdAsync(int id) =>
            _context.DetallesRepuesto
                .Include(dr => dr.OrdenService)
                .Include(dr => dr.Repuesto)
                .FirstOrDefaultAsync(dr => dr.IdDetalle == id);

        public async Task CreateAsync(MDetalleRepuesto detalle)
        {
            _context.DetallesRepuesto.Add(detalle);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MDetalleRepuesto detalle)
        {
            _context.DetallesRepuesto.Update(detalle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.DetallesRepuesto.FindAsync(id);
            if (entidad != null)
            {
                _context.DetallesRepuesto.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
