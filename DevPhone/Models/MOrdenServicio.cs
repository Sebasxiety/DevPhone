using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MOrdenServicio
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Dispositivo))]
        public int DispositivoId { get; set; }

        public MDispositivo? Dispositivo { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<MDetalleRepuesto>? DetallesRepuesto { get; set; }
    }
}
