using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MeterManagement.API.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;
    }
}
