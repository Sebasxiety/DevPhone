using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IOrdenServicioService
    {
        Task<List<MOrdenServicio>> GetAllAsync();
        Task<MOrdenServicio> GetByIdAsync(int id);
        Task CreateAsync(MOrdenServicio orden);
        Task UpdateAsync(MOrdenServicio orden);
        Task DeleteAsync(int id);
    }
}
