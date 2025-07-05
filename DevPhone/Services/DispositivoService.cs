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

        public async Task<IEnumerable<MDispositivo>> GetByClienteAsync(int clienteId)
        {
            return await _context.Dispositivos
                .Where(d => d.ClienteId == clienteId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
