using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<MCliente>> GetAllAsync();
        Task<MCliente?> GetByIdAsync(int id);
        Task AddAsync(MCliente cliente);
        Task UpdateAsync(MCliente cliente);
        Task DeleteAsync(int id);
    }
}
