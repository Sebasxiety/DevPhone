using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MDispositivo
    {
        [Key]
        public int IdDispositivo { get; set; }

        [Required, StringLength(100)]
        public string Serie { get; set; }

        [Required, StringLength(100)]
        public string Modelo { get; set; }

        [Required, StringLength(100)]
        public string Marca { get; set; }

        [Required]
        public DateTime FechaIngreso { get; set; } = DateTime.Now;

        [Required, StringLength(50)]
        public string EstadoActual { get; set; }

        // FK
        [ForeignKey(nameof(Cliente))]
        public int IdCliente { get; set; }
        public virtual MCliente Cliente { get; set; }

        // Navegación
        public virtual ICollection<MOrdenServicio> Ordenes { get; set; }
    }
}
