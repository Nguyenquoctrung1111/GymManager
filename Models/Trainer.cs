using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymManager.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialization { get; set; } // Weight Training, Cardio, Yoga, etc.

        [StringLength(500)]
        public string Bio { get; set; }

        [Range(0, 100)]
        public int Experience { get; set; } // Years of experience

        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<TrainingSession> TrainingSessions { get; set; }
    }
}
