using DevPhone.Models;

namespace DevPhone.Services
{
    public interface IDetalleRepuestoService
    {
        Task<List<MDetalleRepuesto>> GetAllAsync();
        Task<MDetalleRepuesto> GetByIdAsync(int id);
        Task CreateAsync(MDetalleRepuesto detalle);
        Task UpdateAsync(MDetalleRepuesto detalle);
        Task DeleteAsync(int id);
    }
}
