using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevPhone.Models.Enums;

namespace DevPhone.Models
{
    public class MUsuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, StringLength(100)]
        public string Nombres { get; set; }

        [Required, StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required, StringLength(255)]
        public string Contrasena { get; set; }

        [Required, StringLength(50)]
        public string Rol { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades adicionales para JWT
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Propiedad helper para trabajar con el enum
        [NotMapped]
        public UserRole RoleEnum
        {
            get => UserRoleExtensions.FromString(Rol);
            set => Rol = value.ToDisplayString();
        }

        // Navegaci�n
        [ValidateNever]
        public virtual ICollection<MOrdenServicio> Ordenes { get; set; } = new List<MOrdenServicio>();
    }
}
