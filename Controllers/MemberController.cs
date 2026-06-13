using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManager.Data;
using GymManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Member")]
    public class MemberController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<MemberController> _logger;

        public MemberController(GymDbContext context, ILogger<MemberController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Get current member info
            var members = await _context.Members
                .Include(m => m.User)
                .Include(m => m.Package)
                .ToListAsync();

            return View(members);
        }

        public async Task<IActionResult> ViewSchedule()
        {
            var classes = await _context.Classes
                .Include(c => c.Trainer)
                .ThenInclude(t => t.User)
                .Where(c => c.Status == "Scheduled")
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> ViewMembers()
        {
            var members = await _context.Members
                .Include(m => m.User)
                .ToListAsync();

            return View(members);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterClass(int classId)
        {
            // Get current member
            var members = await _context.Members.FirstOrDefaultAsync();

            if (members != null)
            {
                var enrollment = new ClassEnrollment
                {
                    MemberId = members.Id,
                    ClassId = classId,
                    EnrollmentDate = DateTime.Now,
                    Status = "Active"
                };

                _context.Add(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewSchedule));
        }

        public async Task<IActionResult> ViewPackages()
        {
            var packages = await _context.Packages
                .Where(p => p.IsActive)
                .ToListAsync();

            return View(packages);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterPackage(int packageId)
        {
            var package = await _context.Packages.FindAsync(packageId);
            if (package != null)
            {
                var member = await _context.Members.FirstOrDefaultAsync();
                if (member != null)
                {
                    member.PackageId = packageId;
                    member.JoinDate = DateTime.Now;
                    member.ExpiryDate = DateTime.Now.AddDays(package.DurationDays);
                    member.MembershipStatus = "Active";

                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> ViewTrainingSchedule()
        {
            var sessions = await _context.TrainingSessions
                .Include(ts => ts.Trainer)
                .ThenInclude(t => t.User)
                .Where(ts => ts.Status == "Scheduled")
                .ToListAsync();

            return View(sessions);
        }

        [HttpPost]
        public async Task<IActionResult> BookTrainingSession(int trainerId)
        {
            var member = await _context.Members.FirstOrDefaultAsync();
            var trainer = await _context.Trainers.FindAsync(trainerId);

            if (member != null && trainer != null)
            {
                var session = new TrainingSession
                {
                    TrainerId = trainerId,
                    MemberId = member.Id,
                    SessionDate = DateTime.Now,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now.AddHours(1),
                    Status = "Scheduled"
                };

                _context.Add(session);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewTrainingSchedule));
        }
    }
}
