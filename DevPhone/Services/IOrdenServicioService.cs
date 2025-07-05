using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IOrdenServicioService
    {
        Task<IEnumerable<MOrdenServicio>> GetAllAsync();
        Task<MOrdenServicio?> GetByIdAsync(int id);
    }
}
