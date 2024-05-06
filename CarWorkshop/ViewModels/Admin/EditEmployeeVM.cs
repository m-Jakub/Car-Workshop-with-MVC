using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CarWorkshop.ViewModels.Admin
{
    public class EditEmployeeVM
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required]
        [Range(0, 1000)]
        public decimal HourlyRate { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}