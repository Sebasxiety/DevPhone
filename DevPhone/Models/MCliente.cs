using System.ComponentModel.DataAnnotations;

namespace DevPhone.Models
{
    public class MCliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        public ICollection<MDispositivo>? Dispositivos { get; set; }
    }
}
