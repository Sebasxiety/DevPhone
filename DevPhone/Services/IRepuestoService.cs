using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IRepuestoService
    {
        Task<List<MRepuesto>> GetAllAsync();
        Task<MRepuesto> GetByIdAsync(int id);
        Task CreateAsync(MRepuesto repuesto);
        Task UpdateAsync(MRepuesto repuesto);
        Task DeleteAsync(int id);
    }
}
