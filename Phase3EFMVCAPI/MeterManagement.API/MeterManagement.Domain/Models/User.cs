using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MeterManagement.Domain.Models
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        //public List<RefreshTokenModel> RefreshTokens { get; set; } = new();
    }
}
