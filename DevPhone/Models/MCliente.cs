using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace DevPhone.Models
{
    public class MCliente
    {
        [Key]
        public int IdCliente { get; set; }

        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, StringLength(100)]
        public string Apellido { get; set; }

        [Required, StringLength(20)]
        public string Cedula { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(255), EmailAddress]
        public string Correo { get; set; }

        [StringLength(250)]
        public string Direccion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ValidateNever]
        public virtual ICollection<MDispositivo> Dispositivos { get; set; } = new List<MDispositivo>();
        [ValidateNever]
        public virtual ICollection<MOrdenServicio> Ordenes { get; set; } = new List<MOrdenServicio>();
    }
}
