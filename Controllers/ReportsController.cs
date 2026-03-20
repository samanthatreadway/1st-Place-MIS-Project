using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Services;

namespace fa25group23final.Controllers
{
    [Authorize(Roles = "Admin")] // Only admin can access reports
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;
        private readonly AppDbContext _context;

        public ReportsController(ReportService reportService, AppDbContext context)
        {
            _reportService = reportService;
            _context = context;
        }

        /// <summary>
        /// Report index/selector page
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        #region Report A - All Books Sold

        /// <summary>
        /// Report A: All books sold
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BooksSold([FromQuery] ReportFilters filters)
        {
            // Populate filter dropdowns
            await PopulateBooksSoldDropdowns();

            var report = await _reportService.GetBooksSoldReport(filters);
            return View(report);
        }

        private async Task PopulateBooksSoldDropdowns()
        {
            // Books dropdown
            ViewBag.Books = new SelectList(
                await _context.Books
                    .OrderBy(b => b.Title)
                    .Select(b => new { b.BookID, b.Title })
                    .ToListAsync(),
                "BookID",
                "Title"
            );

            // Genres dropdown
            ViewBag.Genres = new SelectList(
                await _context.Genres
                    .OrderBy(g => g.GenreName)
                    .ToListAsync(),
                "GenreID",
                "GenreName"
            );

            // Customers dropdown
            ViewBag.Customers = new SelectList(
                await _context.Users
                    .OrderBy(u => u.LastName)
                    .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                    .ToListAsync(),
                "Id",
                "FullName"
            );

            // Sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "date-desc", Text = "Most Recent First" },
                new { Value = "date-asc", Text = "Oldest First" },
                new { Value = "profit-desc", Text = "Profit Margin (Descending)" },
                new { Value = "profit-asc", Text = "Profit Margin (Ascending)" },
                new { Value = "price-desc", Text = "Price (Descending)" },
                new { Value = "price-asc", Text = "Price (Ascending)" },
                new { Value = "quantity-desc", Text = "Most Popular (Quantity)" }
            }, "Value", "Text");
        }

        #endregion

        #region Report B - All Orders

        /// <summary>
        /// Report B: All orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Orders([FromQuery] ReportFilters filters)
        {
            await PopulateOrdersDropdowns();

            var report = await _reportService.GetOrdersReport(filters);
            return View(report);
        }

        private async Task PopulateOrdersDropdowns()
        {
            // Customers dropdown
            ViewBag.Customers = new SelectList(
                await _context.Users
                    .OrderBy(u => u.LastName)
                    .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                    .ToListAsync(),
                "Id",
                "FullName"
            );

            // Sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "date-desc", Text = "Most Recent First" },
                new { Value = "date-asc", Text = "Oldest First" },
                new { Value = "profit-desc", Text = "Profit Margin (Descending)" },
                new { Value = "profit-asc", Text = "Profit Margin (Ascending)" },
                new { Value = "price-desc", Text = "Order Total (Descending)" },
                new { Value = "price-asc", Text = "Order Total (Ascending)" }
            }, "Value", "Text");
        }

        #endregion

        #region Report C - All Customers

        /// <summary>
        /// Report C: All customers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Customers([FromQuery] ReportFilters filters)
        {
            PopulateCustomersDropdowns();

            var report = await _reportService.GetCustomersReport(filters);
            return View(report);
        }

        private void PopulateCustomersDropdowns()
        {
            // Sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "revenue-desc", Text = "Revenue (Descending)" },
                new { Value = "revenue-asc", Text = "Revenue (Ascending)" },
                new { Value = "profit-desc", Text = "Profit Margin (Descending)" },
                new { Value = "profit-asc", Text = "Profit Margin (Ascending)" },
                new { Value = "name-asc", Text = "Name (A-Z)" },
                new { Value = "name-desc", Text = "Name (Z-A)" }
            }, "Value", "Text");
        }

        #endregion

        #region Report D - Totals

        /// <summary>
        /// Report D: Totals (revenue, cost, profit)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Totals([FromQuery] ReportFilters filters)
        {
            var report = await _reportService.GetTotalsReport(filters);
            return View(report);
        }

        #endregion

        #region Report E - Current Inventory

        /// <summary>
        /// Report E: Current inventory
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Inventory([FromQuery] ReportFilters filters)
        {
            await PopulateInventoryDropdowns();

            var report = await _reportService.GetInventoryReport(filters);
            return View(report);
        }

        private async Task PopulateInventoryDropdowns()
        {
            // Genres dropdown
            ViewBag.Genres = new SelectList(
                await _context.Genres
                    .OrderBy(g => g.GenreName)
                    .ToListAsync(),
                "GenreID",
                "GenreName"
            );

            // Sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "title-asc", Text = "Title (A-Z)" },
                new { Value = "title-desc", Text = "Title (Z-A)" },
                new { Value = "quantity-desc", Text = "Quantity (Descending)" },
                new { Value = "quantity-asc", Text = "Quantity (Ascending)" },
                new { Value = "value-desc", Text = "Total Value (Descending)" },
                new { Value = "value-asc", Text = "Total Value (Ascending)" }
            }, "Value", "Text");
        }

        #endregion

        #region Report F - Employee Reviews

        /// <summary>
        /// Report F: Employee review approvals/rejections
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EmployeeReviews([FromQuery] ReportFilters filters)
        {
            await PopulateEmployeeReviewsDropdowns();

            var report = await _reportService.GetEmployeeReviewsReport(filters);
            return View(report);
        }

        private async Task PopulateEmployeeReviewsDropdowns()
        {
            // Get employees who have approved/rejected reviews
            var employees = await _context.Reviews
                .Include(r => r.approver)
                .Where(r => r.approver != null)
                .Select(r => r.approver)
                .Distinct()
                .OrderBy(u => u.LastName)
                .Select(u => new { u.Id, FullName = u.FirstName + " " + u.LastName })
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "Id", "FullName");

            // Sort options
            ViewBag.SortOptions = new SelectList(new[]
            {
                new { Value = "empid-asc", Text = "Employee ID (Ascending)" },
                new { Value = "empid-desc", Text = "Employee ID (Descending)" },
                new { Value = "approved-desc", Text = "Approved Reviews (Descending)" },
                new { Value = "approved-asc", Text = "Approved Reviews (Ascending)" },
                new { Value = "rejected-desc", Text = "Rejected Reviews (Descending)" },
                new { Value = "rejected-asc", Text = "Rejected Reviews (Ascending)" },
                new { Value = "total-desc", Text = "Total Reviews (Descending)" },
                new { Value = "total-asc", Text = "Total Reviews (Ascending)" }
            }, "Value", "Text");

            // Status filter
            ViewBag.StatusOptions = new SelectList(new[]
            {
                new { Value = "all", Text = "All" },
                new { Value = "approved", Text = "Approved Only" },
                new { Value = "rejected", Text = "Rejected Only" }
            }, "Value", "Text");
        }

        #endregion
    }
}
