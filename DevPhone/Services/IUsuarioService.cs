using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IUsuarioService
    {
        Task<List<MUsuario>> GetAllAsync();
        Task<MUsuario> GetByIdAsync(int id);
        Task CreateAsync(MUsuario usuario);
        Task UpdateAsync(MUsuario usuario);
        Task DeleteAsync(int id);
    }
}
