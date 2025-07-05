using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevPhone.Models
{
    public class MDispositivo
    {
        public int Id { get; set; }

        [Required]
        public string Modelo { get; set; } = string.Empty;

        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }

        public MCliente? Cliente { get; set; }
    }
}
