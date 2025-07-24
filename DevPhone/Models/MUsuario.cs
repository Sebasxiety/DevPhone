using System.ComponentModel.DataAnnotations;

namespace DevPhone.Models
{
    public class MUsuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required, StringLength(255)]
        public string Contrasena { get; set; }

        [Required, StringLength(50)]
        public string Rol { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        public virtual ICollection<MOrdenServicio> Ordenes { get; set; }
    }
}
