using System.ComponentModel.DataAnnotations;

namespace DevPhone.Models.DTOs
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede exceder los 100 caracteres")]
        public string FullName { get; set; } = string.Empty;
    }

    public class UpdateProfileResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UpdatedFullName { get; set; }
    }
}