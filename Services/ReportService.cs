using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;

namespace fa25group23final.Services
{
    /// <summary>
    /// Service for generating all reports - centralized business logic
    /// </summary>
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        #region Report A - All Books Sold

        /// <summary>
        /// Report A: Get all books sold with transaction-level detail
        /// Sorting: most recent, profit margin (asc/desc), price (asc/desc), most popular
        /// </summary>
        public async Task<BooksSoldReportViewModel> GetBooksSoldReport(ReportFilters filters)
        {
            // Start with completed orders only
            var query = _context.OrderDetails
                .Include(od => od.bookID)
                    .ThenInclude(b => b.genre)
                .Include(od => od.orderID)
                    .ThenInclude(o => o.customerID)
                .Where(od => od.orderID.OrderStatus == true);

            // Apply filters
            if (filters.StartDate.HasValue)
                query = query.Where(od => od.orderID.OrderDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(od => od.orderID.OrderDate <= filters.EndDate.Value);

            if (filters.BookId.HasValue)
                query = query.Where(od => od.bookID.BookID == filters.BookId.Value);

            if (!string.IsNullOrEmpty(filters.CustomerId))
                query = query.Where(od => od.orderID.customerID.Id == filters.CustomerId);

            if (filters.GenreId.HasValue)
                query = query.Where(od => od.bookID.genre.GenreID == filters.GenreId.Value);

            if (filters.MinPrice.HasValue)
                query = query.Where(od => od.Price >= filters.MinPrice.Value);

            if (filters.MaxPrice.HasValue)
                query = query.Where(od => od.Price <= filters.MaxPrice.Value);

            // Get total count before pagination
            int totalRecords = await query.CountAsync();

            // Apply sorting
            query = filters.SortBy switch
            {
                "date-asc" => query.OrderBy(od => od.orderID.OrderDate),
                "date-desc" => query.OrderByDescending(od => od.orderID.OrderDate),
                "profit-asc" => query.OrderBy(od => (od.Price - od.Cost)),
                "profit-desc" => query.OrderByDescending(od => (od.Price - od.Cost)),
                "price-asc" => query.OrderBy(od => od.Price),
                "price-desc" => query.OrderByDescending(od => od.Price),
                "quantity-desc" => query.OrderByDescending(od => od.Quantity),
                _ => query.OrderByDescending(od => od.orderID.OrderDate) // Default: most recent
            };

            // Apply pagination
            var results = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            // Map to ViewModel
            var viewModel = new BooksSoldReportViewModel
            {
                Filters = filters,
                TotalRecords = totalRecords,
                CurrentPage = filters.PageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize),
                Results = results.Select(od => new BooksSoldReportViewModel.BookSoldRow
                {
                    OrderDetailId = od.OrderDetailsID,
                    OrderNumber = od.orderID.OrderID,
                    OrderDate = od.orderID.OrderDate ?? DateTime.MinValue,
                    BookTitle = od.bookID.Title,
                    Quantity = od.Quantity,
                    CustomerName = $"{od.orderID.customerID.FirstName} {od.orderID.customerID.LastName}",
                    SellingPrice = od.Price,
                    WeightedAvgCost = od.Cost,
                    ProfitAmount = (od.Price - od.Cost),
                    //ProfitAmount = (od.Price - od.Cost) * od.Quantity,
                    ProfitMarginPercent = od.Price != 0 ? ((od.Price - od.Cost) / od.Price * 100) : 0
                }).ToList()
            };

            return viewModel;
        }

        #endregion

        #region Report B - All Orders

        /// <summary>
        /// Report B: Get all orders grouped by order
        /// Sorting: most recent, profit margin (asc/desc), price (asc/desc)
        /// </summary>
        public async Task<OrdersReportViewModel> GetOrdersReport(ReportFilters filters)
        {
            // Start with completed orders
            var query = _context.Orders
                .Include(o => o.customerID)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderStatus == true);

            // Apply filters
            if (filters.StartDate.HasValue)
                query = query.Where(o => o.OrderDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(o => o.OrderDate <= filters.EndDate.Value);

            if (!string.IsNullOrEmpty(filters.CustomerId))
                query = query.Where(o => o.customerID.Id == filters.CustomerId);

            if (filters.OrderId.HasValue)
                query = query.Where(o => o.OrderID == filters.OrderId.Value);

            // Get total count before pagination
            int totalRecords = await query.CountAsync();

            // Execute query
            var orders = await query.ToListAsync();

            // Calculate order-level metrics
            var orderRows = orders.Select(o => new OrdersReportViewModel.OrderRow
            {
                OrderNumber = o.OrderID,
                OrderDate = o.OrderDate ?? DateTime.MinValue,
                CustomerName = $"{o.customerID.FirstName} {o.customerID.LastName}",
                TotalQuantity = o.OrderDetails.Sum(od => od.Quantity),
                OrderTotal = o.OrderDetails.Sum(od => od.Price * od.Quantity),
                TotalCost = o.OrderDetails.Sum(od => od.Cost * od.Quantity),
                Profit = o.OrderDetails.Sum(od => (od.Price - od.Cost) * od.Quantity),
                ProfitMarginPercent = 0 // Will calculate below
            }).ToList();

            // Calculate profit margin for each order
            foreach (var row in orderRows)
            {
                row.ProfitMarginPercent = row.OrderTotal != 0 
                    ? (row.Profit / row.OrderTotal * 100) 
                    : 0;
            }

            // Apply profit margin filter if specified
            if (filters.MinProfitMargin.HasValue)
                orderRows = orderRows.Where(r => r.ProfitMarginPercent >= filters.MinProfitMargin.Value).ToList();

            if (filters.MaxProfitMargin.HasValue)
                orderRows = orderRows.Where(r => r.ProfitMarginPercent <= filters.MaxProfitMargin.Value).ToList();

            // Apply price filters
            if (filters.MinPrice.HasValue)
                orderRows = orderRows.Where(r => r.OrderTotal >= filters.MinPrice.Value).ToList();

            if (filters.MaxPrice.HasValue)
                orderRows = orderRows.Where(r => r.OrderTotal <= filters.MaxPrice.Value).ToList();

            // Update total records after filtering
            totalRecords = orderRows.Count;

            // Apply sorting
            orderRows = filters.SortBy switch
            {
                "date-asc" => orderRows.OrderBy(r => r.OrderDate).ToList(),
                "date-desc" => orderRows.OrderByDescending(r => r.OrderDate).ToList(),
                "profit-asc" => orderRows.OrderBy(r => r.Profit).ToList(),
                "profit-desc" => orderRows.OrderByDescending(r => r.Profit).ToList(),
                "price-asc" => orderRows.OrderBy(r => r.OrderTotal).ToList(),
                "price-desc" => orderRows.OrderByDescending(r => r.OrderTotal).ToList(),
                _ => orderRows.OrderByDescending(r => r.OrderDate).ToList()
            };

            // Apply pagination
            var paginatedResults = orderRows
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new OrdersReportViewModel
            {
                Filters = filters,
                TotalRecords = totalRecords,
                CurrentPage = filters.PageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize),
                Results = paginatedResults
            };
        }

        #endregion

        #region Report C - All Customers

        /// <summary>
        /// Report C: Get all customers with aggregated purchase data
        /// Sorting: profit margin (asc/desc), revenue (asc/desc)
        /// </summary>
        public async Task<CustomersReportViewModel> GetCustomersReport(ReportFilters filters)
        {
            // Get all customers with completed orders
            var customersWithOrders = await _context.Orders
                .Include(o => o.customerID)
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderStatus == true)
                .GroupBy(o => o.customerID)
                .Select(g => new
                {
                    Customer = g.Key,
                    Orders = g.ToList()
                })
                .ToListAsync();

            // Calculate customer-level metrics
            var customerRows = customersWithOrders.Select(c => new CustomersReportViewModel.CustomerRow
            {
                CustomerId = c.Customer.Id,
                CustomerName = $"{c.Customer.FirstName} {c.Customer.LastName}",
                Email = c.Customer.Email,
                TotalOrders = c.Orders.Count,
                TotalItemsPurchased = c.Orders.SelectMany(o => o.OrderDetails).Sum(od => od.Quantity),
                TotalRevenue = c.Orders.SelectMany(o => o.OrderDetails).Sum(od => od.Price * od.Quantity),
                TotalCost = c.Orders.SelectMany(o => o.OrderDetails).Sum(od => od.Cost * od.Quantity),
                TotalProfit = c.Orders.SelectMany(o => o.OrderDetails).Sum(od => (od.Price - od.Cost) * od.Quantity),
                ProfitMarginPercent = 0 // Will calculate below
            }).ToList();

            // Calculate profit margin for each customer
            foreach (var row in customerRows)
            {
                row.ProfitMarginPercent = row.TotalRevenue != 0 
                    ? (row.TotalProfit / row.TotalRevenue * 100) 
                    : 0;
            }

            // Apply filters
            if (filters.MinProfitMargin.HasValue)
                customerRows = customerRows.Where(r => r.ProfitMarginPercent >= filters.MinProfitMargin.Value).ToList();

            if (filters.MaxProfitMargin.HasValue)
                customerRows = customerRows.Where(r => r.ProfitMarginPercent <= filters.MaxProfitMargin.Value).ToList();

            if (filters.MinPrice.HasValue) // Using MinPrice as MinRevenue
                customerRows = customerRows.Where(r => r.TotalRevenue >= filters.MinPrice.Value).ToList();

            if (filters.MaxPrice.HasValue) // Using MaxPrice as MaxRevenue
                customerRows = customerRows.Where(r => r.TotalRevenue <= filters.MaxPrice.Value).ToList();

            // Get total count
            int totalRecords = customerRows.Count;

            // Apply sorting
            customerRows = filters.SortBy switch
            {
                "profit-asc" => customerRows.OrderBy(r => r.TotalProfit).ToList(),
                "profit-desc" => customerRows.OrderByDescending(r => r.TotalProfit).ToList(),
                "revenue-asc" => customerRows.OrderBy(r => r.TotalRevenue).ToList(),
                "revenue-desc" => customerRows.OrderByDescending(r => r.TotalRevenue).ToList(),
                "name-asc" => customerRows.OrderBy(r => r.CustomerName).ToList(),
                "name-desc" => customerRows.OrderByDescending(r => r.CustomerName).ToList(),
                _ => customerRows.OrderByDescending(r => r.TotalRevenue).ToList()
            };

            // Apply pagination
            var paginatedResults = customerRows
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new CustomersReportViewModel
            {
                Filters = filters,
                TotalRecords = totalRecords,
                CurrentPage = filters.PageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize),
                Results = paginatedResults
            };
        }

        #endregion

        #region Report D - Totals

        /// <summary>
        /// Report D: Get total revenue, cost, and profit (just 3 numbers)
        /// </summary>
        public async Task<TotalsReportViewModel> GetTotalsReport(ReportFilters filters)
        {
            // Get all completed order details
            var query = _context.OrderDetails
                .Include(od => od.orderID)
                .Where(od => od.orderID.OrderStatus == true);

            // Apply date filters if specified
            if (filters.StartDate.HasValue)
                query = query.Where(od => od.orderID.OrderDate >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(od => od.orderID.OrderDate <= filters.EndDate.Value);

            var orderDetails = await query.ToListAsync();

            decimal totalRevenue = orderDetails.Sum(od => od.Price * od.Quantity);
            decimal totalCost = orderDetails.Sum(od => od.Cost * od.Quantity);
            decimal totalProfit = totalRevenue - totalCost;

            return new TotalsReportViewModel
            {
                Filters = filters,
                TotalRecords = orderDetails.Count,
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                TotalProfit = totalProfit,
                ProfitMarginPercent = totalRevenue != 0 ? (totalProfit / totalRevenue * 100) : 0
            };
        }

        #endregion

        #region Report E - Current Inventory

        /// <summary>
        /// Report E: Get current inventory with total value
        /// </summary>
        public async Task<InventoryReportViewModel> GetInventoryReport(ReportFilters filters)
        {
            // Get books with inventory > 0
            var query = _context.Books
                .Include(b => b.genre)
                .Where(b => b.InventoryQuantity > 0);

            // Apply filters
            if (filters.BookId.HasValue)
                query = query.Where(b => b.BookID == filters.BookId.Value);

            if (filters.GenreId.HasValue)
                query = query.Where(b => b.genre.GenreID == filters.GenreId.Value);

            // Get total count
            int totalRecords = await query.CountAsync();

            // Apply sorting
            query = filters.SortBy switch
            {
                "title-asc" => query.OrderBy(b => b.Title),
                "title-desc" => query.OrderByDescending(b => b.Title),
                "quantity-asc" => query.OrderBy(b => b.InventoryQuantity),
                "quantity-desc" => query.OrderByDescending(b => b.InventoryQuantity),
                "value-asc" => query.OrderBy(b => b.InventoryQuantity * b.BookCost),
                "value-desc" => query.OrderByDescending(b => b.InventoryQuantity * b.BookCost),
                _ => query.OrderBy(b => b.Title)
            };

            // Apply pagination
            var books = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            // Map to rows
            var inventoryRows = books.Select(b => new InventoryReportViewModel.InventoryRow
            {
                BookId = b.BookID,
                BookTitle = b.Title,
                Authors = b.Authors,
                GenreName = b.genre?.GenreName ?? "N/A",
                InventoryQuantity = b.InventoryQuantity,
                WeightedAvgCost = b.BookCost,
                TotalValue = b.InventoryQuantity * b.BookCost
            }).ToList();

            // Calculate total inventory value (all books, not just current page)
            decimal totalInventoryValue = await _context.Books
                .Where(b => b.InventoryQuantity > 0)
                .SumAsync(b => b.InventoryQuantity * b.BookCost);

            return new InventoryReportViewModel
            {
                Filters = filters,
                TotalRecords = totalRecords,
                CurrentPage = filters.PageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize),
                Results = inventoryRows,
                TotalInventoryValue = totalInventoryValue
            };
        }

        #endregion

        #region Report F - Employee Reviews

        /// <summary>
        /// Report F: Get approved/rejected reviews by employee
        /// Sorting: EmpID (asc), approved count (asc/desc), rejected count (asc/desc)
        /// </summary>
        public async Task<EmployeeReviewsReportViewModel> GetEmployeeReviewsReport(ReportFilters filters)
        {
            // Get all reviews that have been approved or rejected (not null)
            var reviews = await _context.Reviews
                .Include(r => r.approver)
                .Where(r => r.DisputeStatus != null && r.approver != null)
                .ToListAsync();

            // Group by employee
            var employeeGroups = reviews
                .GroupBy(r => r.approver)
                .Select(g => new EmployeeReviewsReportViewModel.EmployeeReviewRow
                {
                    EmployeeId = g.Key.Id,
                    EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                    Email = g.Key.Email,
                    ApprovedCount = g.Count(r => r.DisputeStatus == true),
                    RejectedCount = g.Count(r => r.DisputeStatus == false),
                    TotalReviews = g.Count(),
                    ApprovalRate = 0 // Will calculate below
                })
                .ToList();

            // Calculate approval rate
            foreach (var row in employeeGroups)
            {
                row.ApprovalRate = row.TotalReviews > 0 
                    ? (row.ApprovedCount / (decimal)row.TotalReviews * 100) 
                    : 0;
            }

            // Apply filters
            if (!string.IsNullOrEmpty(filters.EmployeeId))
                employeeGroups = employeeGroups.Where(r => r.EmployeeId == filters.EmployeeId).ToList();

            if (!string.IsNullOrEmpty(filters.ReviewStatus))
            {
                if (filters.ReviewStatus == "approved")
                    employeeGroups = employeeGroups.Where(r => r.ApprovedCount > 0).ToList();
                else if (filters.ReviewStatus == "rejected")
                    employeeGroups = employeeGroups.Where(r => r.RejectedCount > 0).ToList();
            }

            // Get total count
            int totalRecords = employeeGroups.Count;

            // Apply sorting
            employeeGroups = filters.SortBy switch
            {
                "empid-asc" => employeeGroups.OrderBy(r => r.EmployeeId).ToList(),
                "empid-desc" => employeeGroups.OrderByDescending(r => r.EmployeeId).ToList(),
                "approved-asc" => employeeGroups.OrderBy(r => r.ApprovedCount).ToList(),
                "approved-desc" => employeeGroups.OrderByDescending(r => r.ApprovedCount).ToList(),
                "rejected-asc" => employeeGroups.OrderBy(r => r.RejectedCount).ToList(),
                "rejected-desc" => employeeGroups.OrderByDescending(r => r.RejectedCount).ToList(),
                "total-asc" => employeeGroups.OrderBy(r => r.TotalReviews).ToList(),
                "total-desc" => employeeGroups.OrderByDescending(r => r.TotalReviews).ToList(),
                _ => employeeGroups.OrderBy(r => r.EmployeeId).ToList()
            };

            // Apply pagination
            var paginatedResults = employeeGroups
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            return new EmployeeReviewsReportViewModel
            {
                Filters = filters,
                TotalRecords = totalRecords,
                CurrentPage = filters.PageNumber,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)filters.PageSize),
                Results = paginatedResults
            };
        }

        #endregion

        #region Book Profit Margin Calculation

        /// <summary>
        /// Calculate average profit margin per book
        /// Logic: For each order, calculate (selling price - avg cost) * quantity
        /// Sum all profits, divide by total quantity sold
        /// </summary>
        public async Task<decimal?> GetAverageProfitMarginPerBook(int bookId)
        {
            // Get all completed order details for this book
            var orderDetails = await _context.OrderDetails
                .Include(od => od.orderID)
                .Where(od => od.bookID.BookID == bookId && od.orderID.OrderStatus == true)
                .Select(od => new
                {
                    ProfitPerUnit = od.Price - od.Cost,  // Selling price - avg cost at time of purchase
                    Quantity = od.Quantity
                })
                .ToListAsync();

            // If no sales data, return null
            if (!orderDetails.Any())
            {
                return null;
            }

            // Calculate total profit across all orders
            decimal totalProfit = orderDetails.Sum(od => od.ProfitPerUnit * od.Quantity);

            // Calculate total quantity sold across all orders
            int totalQuantitySold = orderDetails.Sum(od => od.Quantity);

            // Calculate and return average profit margin per book
            decimal averageProfitMargin = totalProfit / totalQuantitySold;

            return averageProfitMargin;
        }

        #endregion
    }
}
