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

        public async Task<MUsuario?> LoginAsync(string username, string password)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == username && u.Password == password);
        }
    }
}
