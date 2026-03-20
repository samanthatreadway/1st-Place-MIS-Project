using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
    /// <summary>
    /// Base class for all report ViewModels - contains shared properties
    /// </summary>
    public abstract class BaseReportViewModel
    {
        public ReportFilters Filters { get; set; } = new ReportFilters();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public DateTime ReportGeneratedAt { get; set; } = DateTime.Now;

        // Helper property for pagination
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    #region Report A - All Books Sold

    /// <summary>
    /// Report A: All books sold - transaction-level detail
    /// </summary>
    public class BooksSoldReportViewModel : BaseReportViewModel
    {
        public List<BookSoldRow> Results { get; set; } = new List<BookSoldRow>();

        public class BookSoldRow
        {
            public int OrderDetailId { get; set; }
            
            [Display(Name = "Order #")]
            public int OrderNumber { get; set; }
            
            [Display(Name = "Order Date")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime OrderDate { get; set; }
            
            [Display(Name = "Book Title")]
            public string BookTitle { get; set; }
            
            [Display(Name = "Quantity")]
            public int Quantity { get; set; }
            
            [Display(Name = "Customer")]
            public string CustomerName { get; set; }
            
            [Display(Name = "Selling Price")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal SellingPrice { get; set; }
            
            [Display(Name = "Weighted Avg Cost")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal WeightedAvgCost { get; set; }
            
            [Display(Name = "Profit Margin %")]
            [DisplayFormat(DataFormatString = "{0:F2}%")]
            public decimal ProfitMarginPercent { get; set; }
            
            [Display(Name = "Profit Amount")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal ProfitAmount { get; set; }
        }
    }

    #endregion

    #region Report B - All Orders

    /// <summary>
    /// Report B: All orders - grouped by order
    /// </summary>
    public class OrdersReportViewModel : BaseReportViewModel
    {
        public List<OrderRow> Results { get; set; } = new List<OrderRow>();

        public class OrderRow
        {
            [Display(Name = "Order #")]
            public int OrderNumber { get; set; }
            
            [Display(Name = "Order Date")]
            [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
            public DateTime OrderDate { get; set; }
            
            [Display(Name = "Customer")]
            public string CustomerName { get; set; }
            
            [Display(Name = "Total Items")]
            public int TotalQuantity { get; set; }
            
            [Display(Name = "Order Total")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal OrderTotal { get; set; }
            
            [Display(Name = "Total Cost")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal TotalCost { get; set; }
            
            [Display(Name = "Profit")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal Profit { get; set; }
            
            [Display(Name = "Profit Margin %")]
            [DisplayFormat(DataFormatString = "{0:F2}%")]
            public decimal ProfitMarginPercent { get; set; }
        }
    }

    #endregion

    #region Report C - All Customers

    /// <summary>
    /// Report C: All customers - grouped by customer
    /// </summary>
    public class CustomersReportViewModel : BaseReportViewModel
    {
        public List<CustomerRow> Results { get; set; } = new List<CustomerRow>();

        public class CustomerRow
        {
            public string CustomerId { get; set; }
            
            [Display(Name = "Customer Name")]
            public string CustomerName { get; set; }
            
            [Display(Name = "Email")]
            public string Email { get; set; }
            
            [Display(Name = "Total Orders")]
            public int TotalOrders { get; set; }
            
            [Display(Name = "Total Items Purchased")]
            public int TotalItemsPurchased { get; set; }
            
            [Display(Name = "Total Revenue")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal TotalRevenue { get; set; }
            
            [Display(Name = "Total Cost")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal TotalCost { get; set; }
            
            [Display(Name = "Total Profit")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal TotalProfit { get; set; }
            
            [Display(Name = "Profit Margin %")]
            [DisplayFormat(DataFormatString = "{0:F2}%")]
            public decimal ProfitMarginPercent { get; set; }
        }
    }

    #endregion

    #region Report D - Totals

    /// <summary>
    /// Report D: Totals - just three numbers
    /// </summary>
    public class TotalsReportViewModel : BaseReportViewModel
    {
        [Display(Name = "Total Revenue")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalRevenue { get; set; }
        
        [Display(Name = "Total Cost")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalCost { get; set; }
        
        [Display(Name = "Total Profit")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalProfit { get; set; }
        
        [Display(Name = "Overall Profit Margin %")]
        [DisplayFormat(DataFormatString = "{0:F2}%")]
        public decimal ProfitMarginPercent { get; set; }
    }

    #endregion

    #region Report E - Current Inventory

    /// <summary>
    /// Report E: Current inventory value
    /// </summary>
    public class InventoryReportViewModel : BaseReportViewModel
    {
        public List<InventoryRow> Results { get; set; } = new List<InventoryRow>();
        
        [Display(Name = "Total Inventory Value")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal TotalInventoryValue { get; set; }

        public class InventoryRow
        {
            public int BookId { get; set; }
            
            [Display(Name = "Book Title")]
            public string BookTitle { get; set; }
            
            [Display(Name = "Authors")]
            public string Authors { get; set; }
            
            [Display(Name = "Genre")]
            public string GenreName { get; set; }
            
            [Display(Name = "Quantity in Stock")]
            public int InventoryQuantity { get; set; }
            
            [Display(Name = "Weighted Avg Cost")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal WeightedAvgCost { get; set; }
            
            [Display(Name = "Total Value")]
            [DisplayFormat(DataFormatString = "{0:C}")]
            public decimal TotalValue { get; set; }
        }
    }

    #endregion

    #region Report F - Employee Reviews

    /// <summary>
    /// Report F: Approved/rejected reviews by employee
    /// </summary>
    public class EmployeeReviewsReportViewModel : BaseReportViewModel
    {
        public List<EmployeeReviewRow> Results { get; set; } = new List<EmployeeReviewRow>();

        public class EmployeeReviewRow
        {
            public string EmployeeId { get; set; }
            
            [Display(Name = "Employee Name")]
            public string EmployeeName { get; set; }
            
            [Display(Name = "Email")]
            public string Email { get; set; }
            
            [Display(Name = "Approved Reviews")]
            public int ApprovedCount { get; set; }
            
            [Display(Name = "Rejected Reviews")]
            public int RejectedCount { get; set; }
            
            [Display(Name = "Total Reviews")]
            public int TotalReviews { get; set; }
            
            [Display(Name = "Approval Rate %")]
            [DisplayFormat(DataFormatString = "{0:F2}%")]
            public decimal ApprovalRate { get; set; }
        }
    }

    #endregion
}
