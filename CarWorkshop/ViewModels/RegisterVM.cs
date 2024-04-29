using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CarWorkshop.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
