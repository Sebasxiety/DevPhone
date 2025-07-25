using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MOrdenServicio
    {
        [Key]
        public int IdOrden { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required, StringLength(500)]
        public string DescripcionFalla { get; set; }

        [Required, StringLength(50)]
        public string Estado { get; set; }

        // FKs
        [ForeignKey(nameof(Cliente))]
        public int IdCliente { get; set; }
        [ValidateNever]
        public virtual MCliente Cliente { get; set; }

        [ForeignKey(nameof(Usuario))]
        public int IdUsuario { get; set; }
        [ValidateNever]
        public virtual MUsuario Usuario { get; set; }

        [ForeignKey(nameof(Dispositivo))]
        public int IdDispositivo { get; set; }
        [ValidateNever]
        public virtual MDispositivo Dispositivo { get; set; }

        // Navegación
        [ValidateNever]
        public virtual ICollection<MDetalleRepuesto> DetallesRepuesto { get; set; } = new List<MDetalleRepuesto>();
    }
}
