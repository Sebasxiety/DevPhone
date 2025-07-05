using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IDispositivoService
    {
        Task<IEnumerable<MDispositivo>> GetByClienteAsync(int clienteId);
    }
}
