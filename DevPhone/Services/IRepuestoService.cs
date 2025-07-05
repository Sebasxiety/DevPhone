using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IRepuestoService
    {
        Task<IEnumerable<MRepuesto>> GetAllAsync();
        Task AddAsync(MRepuesto repuesto);
    }
}
