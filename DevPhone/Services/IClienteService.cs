using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IClienteService
    {
        Task<List<MCliente>> GetAllAsync();
        Task<MCliente> GetByIdAsync(int id);
        Task CreateAsync(MCliente cliente);
        Task UpdateAsync(MCliente cliente);
        Task DeleteAsync(int id);
    }
}
