using Azure.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MDetalleRepuesto
    {
        [Key]
        public int IdDetalle { get; set; }

        [Required]
        public int Cantidad { get; set; }

        // FKs
        [ForeignKey(nameof(OrdenService))]
        public int IdOrden { get; set; }
        public virtual MOrdenServicio OrdenService { get; set; }

        [ForeignKey(nameof(Repuesto))]
        public int IdRepuesto { get; set; }
        public virtual MRepuesto Repuesto { get; set; }
    }
}
