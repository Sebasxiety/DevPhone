using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<MUsuario>> GetAllAsync() =>
            _context.Usuarios.ToListAsync();

        public Task<MUsuario> GetByIdAsync(int id) =>
            _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

        public async Task CreateAsync(MUsuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MUsuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entidad = await _context.Usuarios.FindAsync(id);
            if (entidad != null)
            {
                _context.Usuarios.Remove(entidad);
                await _context.SaveChangesAsync();
            }
        }
    }
}
