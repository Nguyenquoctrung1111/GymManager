using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // Cash, Card, Transfer, etc.

        [StringLength(20)]
        public string Status { get; set; } = "Completed"; // Pending, Completed, Failed, Refunded

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string InvoiceNumber { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
    }
}
