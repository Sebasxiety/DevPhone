using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ApplicationDbContext _ctx;
        public ClienteService(ApplicationDbContext ctx) => _ctx = ctx;

        public Task<IEnumerable<MCliente>> GetAllAsync() =>
            _ctx.Clientes.ToListAsync().ContinueWith(t => (IEnumerable<MCliente>)t.Result);

        public Task<MCliente> GetByIdAsync(int id) =>
            _ctx.Clientes.FirstOrDefaultAsync(c => c.IdCliente == id);

        public async Task CreateAsync(MCliente cliente)
        {
            _ctx.Clientes.Add(cliente);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(MCliente cliente)
        {
            _ctx.Clientes.Update(cliente);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _ctx.Clientes.FindAsync(id);
            if (e != null)
            {
                _ctx.Clientes.Remove(e);
                await _ctx.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _ctx.Clientes.AnyAsync(d => d.IdCliente == id);
        }
    }
}
