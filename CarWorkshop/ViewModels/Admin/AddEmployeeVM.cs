using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CarWorkshop.ViewModels.Admin
{
    public class AddEmployeeVM
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public bool IsEdit { get; set; } = false;
    }
}
