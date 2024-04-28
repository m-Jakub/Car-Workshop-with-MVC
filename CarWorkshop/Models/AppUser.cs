using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarWorkshop.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }
    }
}
