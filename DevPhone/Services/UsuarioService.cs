using DevPhone.Models;
using Microsoft.EntityFrameworkCore;

namespace DevPhone.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ApplicationDbContext _context;
        public UsuarioService(ApplicationDbContext context) => _context = context;

        public Task<List<MUsuario>> GetAllAsync() =>
            _context.Usuarios.ToListAsync();

        public Task<MUsuario> GetByIdAsync(int id) =>
            _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

        public Task<MUsuario> ValidateUserAsync(string username, string password) =>
            _context.Usuarios
                    .FirstOrDefaultAsync(u =>
                        u.NombreUsuario == username &&
                        u.Contrasena == password);

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
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(d => d.IdUsuario == id);
        }

        // Métodos adicionales para JWT
        public Task<MUsuario> GetUserByIdAsync(int id) =>
            _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

        public Task<MUsuario> GetUserByRefreshTokenAsync(string refreshToken) =>
            _context.Usuarios.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        public async Task UpdateUserAsync(MUsuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (user == null || user.Contrasena != currentPassword)
            {
                return false;
            }

            user.Contrasena = newPassword;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateProfileAsync(int userId, string fullName)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (user == null)
            {
                return false;
            }

            user.Nombres = fullName;
            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
