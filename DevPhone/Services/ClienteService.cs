using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MCliente>> GetAllAsync() =>
            _context.Clientes.ToListAsync();

        public Task<MCliente> GetByIdAsync(int id) =>
            _context.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);

        public async Task CreateAsync(MCliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MCliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Clientes.FindAsync(id);
            if (entidad != null)
            {
                _context.Clientes.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
