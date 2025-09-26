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

        public DateTime? FechaCompletado { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PrecioServicio { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTotal { get; set; }

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

        // Propiedades calculadas (no mapeadas a la base de datos)
        [NotMapped]
        public decimal SubtotalRepuestos => DetallesRepuesto?.Sum(d => d.Cantidad * d.Repuesto.PrecioUnitario) ?? 0;

        [NotMapped]
        public decimal SubtotalSinIva => PrecioServicio + SubtotalRepuestos;

        [NotMapped]
        public decimal IVA => SubtotalSinIva * 0.15m; // IVA del 15%

        [NotMapped]
        public decimal TotalConIva => SubtotalSinIva + IVA;

        // Navegaci�n
        [ValidateNever]
        public virtual ICollection<MDetalleRepuesto> DetallesRepuesto { get; set; } = new List<MDetalleRepuesto>();

        // M�todo para calcular y actualizar el precio total
        public void CalcularPrecioTotal()
        {
            PrecioTotal = TotalConIva;
        }
    }
}
