using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IUsuarioService
    {
        Task<List<MUsuario>> GetAllAsync();
        Task<MUsuario> GetByIdAsync(int id);
        Task<MUsuario> ValidateUserAsync(string username, string password);
        Task CreateAsync(MUsuario usuario);
        Task UpdateAsync(MUsuario usuario);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
