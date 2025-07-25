using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class DispositivoService : IDispositivoService
    {
        private readonly ApplicationDbContext _context;
        public DispositivoService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Dispositivos.AnyAsync(d => d.IdDispositivo == id);
        }
        public Task<List<MDispositivo>> GetAllAsync() =>
            _context.Dispositivos
                    .Include(d => d.Cliente)
                    .ToListAsync();

        public Task<MDispositivo> GetByIdAsync(int id) =>
            _context.Dispositivos
                    .Include(d => d.Cliente)
                    .FirstOrDefaultAsync(d => d.IdDispositivo == id);

        public async Task CreateAsync(MDispositivo dispositivo)
        {
            _context.Dispositivos.Add(dispositivo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MDispositivo dispositivo)
        {
            _context.Dispositivos.Update(dispositivo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Dispositivos.FindAsync(id);
            if (entidad != null)
            {
                _context.Dispositivos.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
