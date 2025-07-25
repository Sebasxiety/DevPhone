using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IDispositivoService
    {
        Task<List<MDispositivo>> GetAllAsync();
        Task<MDispositivo> GetByIdAsync(int id);
        Task CreateAsync(MDispositivo dispositivo);
        Task UpdateAsync(MDispositivo dispositivo);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
