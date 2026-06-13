using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManager.Data;
using GymManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Trainer")]
    public class TrainerController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<TrainerController> _logger;

        public TrainerController(GymDbContext context, ILogger<TrainerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalClasses = await _context.Classes.CountAsync();
            var totalMembers = await _context.Members.CountAsync();
            var upcomingClasses = await _context.Classes
                .Where(c => c.StartTime > DateTime.Now)
                .OrderBy(c => c.StartTime)
                .Take(5)
                .Include(c => c.Trainer)
                .ToListAsync();

            ViewBag.TotalClasses = totalClasses;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.UpcomingClasses = upcomingClasses;

            return View();
        }

        public async Task<IActionResult> MyClasses()
        {
            var classes = await _context.Classes
                .Include(c => c.Trainer)
                .ThenInclude(t => t.User)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Member)
                .ThenInclude(m => m.User)
                .ToListAsync();

            return View(classes);
        }

        public async Task<IActionResult> ClassDetails(int id)
        {
            var @class = await _context.Classes
                .Include(c => c.Trainer)
                .ThenInclude(t => t.User)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Member)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (@class == null)
                return NotFound();

            return View(@class);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int memberId, int classId, string status)
        {
            var attendance = new Attendance
            {
                MemberId = memberId,
                ClassId = classId,
                CheckInTime = DateTime.Now,
                Status = status
            };

            _context.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<IActionResult> AttendanceReport(int classId)
        {
            var attendances = await _context.Attendances
                .Include(a => a.Member)
                .ThenInclude(m => m.User)
                .Where(a => a.ClassId == classId)
                .ToListAsync();

            return View(attendances);
        }

        public async Task<IActionResult> TrainingSessions()
        {
            var sessions = await _context.TrainingSessions
                .Include(ts => ts.Trainer)
                .ThenInclude(t => t.User)
                .Include(ts => ts.Member)
                .ThenInclude(m => m.User)
                .OrderBy(ts => ts.SessionDate)
                .ToListAsync();

            return View(sessions);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSessionStatus(int sessionId, string status)
        {
            var session = await _context.TrainingSessions.FindAsync(sessionId);
            if (session != null)
            {
                session.Status = status;
                _context.Update(session);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
