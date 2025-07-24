using System.ComponentModel.DataAnnotations;

namespace DevPhone.ViewModels
{
    public class LoginVM
    {
        [Required, Display(Name = "Usuario")]
        public string Username { get; set; }

        [Required, DataType(DataType.Password), Display(Name = "Contraseña")]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
