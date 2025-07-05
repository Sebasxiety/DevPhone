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

        public async Task<IEnumerable<MRepuesto>> GetAllAsync()
        {
            return await _context.Repuestos.AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(MRepuesto repuesto)
        {
            _context.Repuestos.Add(repuesto);
            await _context.SaveChangesAsync();
        }
    }
}
