using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManager.Data;
using GymManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(GymDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalMembers = await _context.Members.CountAsync();
            var totalTrainers = await _context.Trainers.CountAsync();
            var totalPackages = await _context.Packages.CountAsync();
            var totalClasses = await _context.Classes.CountAsync();

            var recentPayments = await _context.Payments
                .OrderByDescending(p => p.PaymentDate)
                .Take(10)
                .Include(p => p.Member)
                .ToListAsync();

            ViewBag.TotalMembers = totalMembers;
            ViewBag.TotalTrainers = totalTrainers;
            ViewBag.TotalPackages = totalPackages;
            ViewBag.TotalClasses = totalClasses;
            ViewBag.RecentPayments = recentPayments;

            return View();
        }

        // Member Management
        public async Task<IActionResult> Members()
        {
            var members = await _context.Members
                .Include(m => m.User)
                .Include(m => m.Package)
                .ToListAsync();
            return View(members);
        }

        // Trainer Management
        public async Task<IActionResult> Trainers()
        {
            var trainers = await _context.Trainers
                .Include(t => t.User)
                .ToListAsync();
            return View(trainers);
        }

        // Package Management
        public async Task<IActionResult> Packages()
        {
            var packages = await _context.Packages.ToListAsync();
            return View(packages);
        }

        public IActionResult CreatePackage()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePackage(Package package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Packages));
            }
            return View(package);
        }

        // Class Management
        public async Task<IActionResult> Classes()
        {
            var classes = await _context.Classes
                .Include(c => c.Trainer)
                .ThenInclude(t => t.User)
                .ToListAsync();
            return View(classes);
        }

        // Payment Management
        public async Task<IActionResult> Payments()
        {
            var payments = await _context.Payments
                .Include(p => p.Member)
                .ThenInclude(m => m.User)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            return View(payments);
        }

        // Reports
        public async Task<IActionResult> Reports()
        {
            var totalRevenue = await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.Amount);

            var memberGrowth = await _context.Members
                .GroupBy(m => m.JoinDate.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Date)
                .Take(30)
                .ToListAsync();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.MemberGrowth = memberGrowth;

            return View();
        }
    }
}
