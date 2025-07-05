using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MDetalleRepuesto
    {
        public int Id { get; set; }

        [ForeignKey(nameof(OrdenServicio))]
        public int OrdenServicioId { get; set; }

        public MOrdenServicio? OrdenServicio { get; set; }

        [ForeignKey(nameof(Repuesto))]
        public int RepuestoId { get; set; }

        public MRepuesto? Repuesto { get; set; }

        public int Cantidad { get; set; }
    }
}
