using System;
using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Utilities;
using System;

namespace fa25group23final.Controllers
{

	public class HomeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public HomeController(AppDbContext appDbContext, UserManager<AppUser> userManager)
		{
			_context = appDbContext;
			_userManager = userManager;

		}

        [Authorize(Roles = "Employee")]
        public IActionResult EmployeeDashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
		{
			return View();
		}

        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons
                .OrderBy(c => c.CouponCode)
                .ToListAsync();

			if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
			{
                // Get the current user
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    // Pass customer's full name to view
                    ViewBag.CustomerFullName = $"{currentUser.FirstName} {currentUser.LastName}";
                }
            }
                return View(coupons);
        }

        public IActionResult Books()
		{
			return RedirectToAction("Index", "Books");
		}

		public async Task<IActionResult> Customers()
		{

			//var usersInRole = await _userManager.GetUsersInRoleAsync("Customer");
			var users = _userManager.Users.ToList();

			return View(users);
		}

		public IActionResult Cards()
		{
			var dbCards = _context.CreditCards.ToList();
			return View(dbCards);
		}

		public async Task<IActionResult> Employees()
		{
			var usersInRole = await _userManager.GetUsersInRoleAsync("Employee");


			return View(usersInRole);

		}

        public IActionResult Reviews()
        {
            var dbReviews = _context.Reviews
                .Include(r => r.reviewer)
                .Include(r => r.approver)   // optional
                .Include(r => r.bookID)     // optional, for showing book title
                .ToList();

            return View(dbReviews);
        }

        public IActionResult Orders()
		{
			var dbOrders = _context.Orders
				.Include(o => o.OrderDetails)
				.ThenInclude( o => o.bookID)
				.Include(o => o.creditCard)
				.Include(o => o.customerID)
				.ToList();

			return View(dbOrders);
		}

        public IActionResult OrderDetails()
        {
            var dbOrderDetails = _context.OrderDetails
				.Include(od => od.bookID)
				.ToList();
            return View(dbOrderDetails);
        }

		public IActionResult UserDashboard()
		{
			return View();
		}

		public IActionResult MeetTheTeam()
		{
			return View();
		}

    }
}