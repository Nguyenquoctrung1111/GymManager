using System.ComponentModel.DataAnnotations;

namespace GymManager.Models
{
    public class Package
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(1, 365)]
        public int DurationDays { get; set; }

        [Range(0, 1000)]
        public int SessionsIncluded { get; set; } // 0 = Unlimited

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<Member> Members { get; set; }
    }
}
