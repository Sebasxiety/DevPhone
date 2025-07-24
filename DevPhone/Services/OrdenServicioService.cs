using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class OrdenServicioService : IOrdenServicioService
    {
        private readonly ApplicationDbContext _context;

        public OrdenServicioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MOrdenServicio>> GetAllAsync() =>
            _context.OrdenesServicio
                .Include(o => o.Cliente)
                .Include(o => o.Usuario)
                .Include(o => o.Dispositivo)
                .ToListAsync();

        public Task<MOrdenServicio> GetByIdAsync(int id) =>
            _context.OrdenesServicio
                .Include(o => o.Cliente)
                .Include(o => o.Usuario)
                .Include(o => o.Dispositivo)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

        public async Task CreateAsync(MOrdenServicio orden)
        {
            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MOrdenServicio orden)
        {
            _context.OrdenesServicio.Update(orden);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.OrdenesServicio.FindAsync(id);
            if (entidad != null)
            {
                _context.OrdenesServicio.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
