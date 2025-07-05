using System.ComponentModel.DataAnnotations;

namespace DevPhone.Models
{
    public class MRepuesto
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }
    }
}
