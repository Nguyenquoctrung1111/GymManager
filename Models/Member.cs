using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int PackageId { get; set; }

        [ForeignKey("PackageId")]
        public virtual Package Package { get; set; }

        public DateTime JoinDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        [StringLength(20)]
        public string MembershipStatus { get; set; } = "Active"; // Active, Expired, Suspended

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<ClassEnrollment> ClassEnrollments { get; set; }
    }
}
