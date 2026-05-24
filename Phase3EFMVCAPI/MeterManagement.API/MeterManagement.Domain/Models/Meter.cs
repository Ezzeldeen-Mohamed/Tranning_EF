using MeterManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MeterManagement.Domain.Models
{
    public class Meter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Serial Number")]
        [MaxLength(100)]
        public string SerialNumber { get; set; }
        public MeterStatus Status { get; set; }

        //3shan n3ml softdelete w nshof etmsa7t emta    
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public string? UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
