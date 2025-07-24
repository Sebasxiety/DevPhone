using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MRepuesto
    {
        [Key]
        public int IdRepuesto { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        // Navegación
        public virtual ICollection<MDetalleRepuesto> DetallesRepuesto { get; set; }
    }
}
