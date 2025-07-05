using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IUsuarioService
    {
        Task<MUsuario?> LoginAsync(string username, string password);
    }
}
