using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using GymManager.Models;

namespace GymManager.Data
{
    public class GymDbContext : IdentityDbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<ClassEnrollment> ClassEnrollments { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Member>()
                .HasOne(m => m.User)
                .WithOne(u => u.Member)
                .HasForeignKey<Member>(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Trainer>()
                .HasOne(t => t.User)
                .WithOne(u => u.Trainer)
                .HasForeignKey<Trainer>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Member)
                .WithMany(m => m.ClassEnrollments)
                .HasForeignKey(ce => ce.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClassEnrollment>()
                .HasOne(ce => ce.Class)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(ce => ce.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Trainer)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Member)
                .WithMany(m => m.Attendances)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Member)
                .WithMany(m => m.Payments)
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainingSessions)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(ts => ts.Member)
                .WithMany()
                .HasForeignKey(ts => ts.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
