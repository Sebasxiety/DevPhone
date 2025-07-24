using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class RepuestoService : IRepuestoService
    {
        private readonly ApplicationDbContext _context;

        public RepuestoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MRepuesto>> GetAllAsync() =>
            _context.Repuestos.ToListAsync();

        public Task<MRepuesto> GetByIdAsync(int id) =>
            _context.Repuestos.FirstOrDefaultAsync(r => r.IdRepuesto == id);

        public async Task CreateAsync(MRepuesto repuesto)
        {
            _context.Repuestos.Add(repuesto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MRepuesto repuesto)
        {
            _context.Repuestos.Update(repuesto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Repuestos.FindAsync(id);
            if (entidad != null)
            {
                _context.Repuestos.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
