using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymManager.Data;
using GymManager.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManager.Controllers
{
    [Authorize(Roles = "Cashier")]
    public class CashierController : Controller
    {
        private readonly GymDbContext _context;
        private readonly ILogger<CashierController> _logger;

        public CashierController(GymDbContext context, ILogger<CashierController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalPayments = await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.Amount);

            var todayPayments = await _context.Payments
                .Where(p => p.PaymentDate.Date == DateTime.Now.Date && p.Status == "Completed")
                .SumAsync(p => p.Amount);

            ViewBag.TotalPayments = totalPayments;
            ViewBag.TodayPayments = todayPayments;

            return View();
        }

        public async Task<IActionResult> CollectPayment()
        {
            var members = await _context.Members
                .Include(m => m.User)
                .ToListAsync();

            return View(members);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessPayment(int memberId, decimal amount, string paymentMethod, string description)
        {
            var member = await _context.Members.FindAsync(memberId);
            if (member != null)
            {
                var payment = new Payment
                {
                    MemberId = memberId,
                    Amount = amount,
                    PaymentMethod = paymentMethod,
                    Status = "Completed",
                    PaymentDate = DateTime.Now,
                    Description = description,
                    InvoiceNumber = GenerateInvoiceNumber()
                };

                _context.Add(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PaymentReceipt), new { paymentId = payment.Id });
            }

            return RedirectToAction(nameof(CollectPayment));
        }

        public async Task<IActionResult> RegisterPackage(int memberId)
        {
            var packages = await _context.Packages
                .Where(p => p.IsActive)
                .ToListAsync();

            ViewBag.MemberId = memberId;
            return View(packages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterPackageForMember(int memberId, int packageId)
        {
            var member = await _context.Members.FindAsync(memberId);
            var package = await _context.Packages.FindAsync(packageId);

            if (member != null && package != null)
            {
                member.PackageId = packageId;
                member.JoinDate = DateTime.Now;
                member.ExpiryDate = DateTime.Now.AddDays(package.DurationDays);
                member.MembershipStatus = "Active";

                var payment = new Payment
                {
                    MemberId = memberId,
                    Amount = package.Price,
                    PaymentMethod = "Not Specified",
                    Status = "Completed",
                    PaymentDate = DateTime.Now,
                    Description = $"Package: {package.Name}",
                    InvoiceNumber = GenerateInvoiceNumber()
                };

                _context.Update(member);
                _context.Add(payment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PaymentReceipt), new { paymentId = payment.Id });
            }

            return RedirectToAction(nameof(RegisterPackage), new { memberId });
        }

        public async Task<IActionResult> RenewPackage(int memberId)
        {
            var member = await _context.Members
                .Include(m => m.Package)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member?.Package != null)
            {
                member.ExpiryDate = DateTime.Now.AddDays(member.Package.DurationDays);
                member.MembershipStatus = "Active";

                var payment = new Payment
                {
                    MemberId = memberId,
                    Amount = member.Package.Price,
                    PaymentMethod = "Not Specified",
                    Status = "Completed",
                    PaymentDate = DateTime.Now,
                    Description = $"Renewal: {member.Package.Name}",
                    InvoiceNumber = GenerateInvoiceNumber()
                };

                _context.Update(member);
                _context.Add(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> PaymentHistory()
        {
            var payments = await _context.Payments
                .Include(p => p.Member)
                .ThenInclude(m => m.User)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            return View(payments);
        }

        public async Task<IActionResult> PaymentReceipt(int paymentId)
        {
            var payment = await _context.Payments
                .Include(p => p.Member)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            return View(payment);
        }

        private string GenerateInvoiceNumber()
        {
            return "INV-" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}
