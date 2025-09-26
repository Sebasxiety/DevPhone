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

        // Métodos adicionales para JWT
        Task<MUsuario> GetUserByIdAsync(int id);
        Task<MUsuario> GetUserByRefreshTokenAsync(string refreshToken);
        Task UpdateUserAsync(MUsuario usuario);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
