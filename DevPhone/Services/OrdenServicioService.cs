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

        public async Task<IEnumerable<MOrdenServicio>> GetAllAsync()
        {
            return await _context.OrdenesServicio
                .Include(o => o.Dispositivo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MOrdenServicio?> GetByIdAsync(int id)
        {
            return await _context.OrdenesServicio
                .Include(o => o.Dispositivo)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
