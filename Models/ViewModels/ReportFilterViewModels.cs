using System;
using System.ComponentModel.DataAnnotations;

namespace fa25group23final.Models
{
    /// <summary>
    /// Filter criteria for all reports
    /// Used for query string parameters and form inputs
    /// </summary>
    public class ReportFilters
    {
        // Date range filters
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Sorting options
        [Display(Name = "Sort By")]
        public string SortBy { get; set; } = "date-desc";

        // Entity filters
        [Display(Name = "Customer")]
        public string? CustomerId { get; set; }

        [Display(Name = "Book")]
        public int? BookId { get; set; }

        [Display(Name = "Genre")]
        public int? GenreId { get; set; }

        [Display(Name = "Order Number")]
        public int? OrderId { get; set; }

        // Price range filters
        [Display(Name = "Min Price")]
        [DataType(DataType.Currency)]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        [DataType(DataType.Currency)]
        public decimal? MaxPrice { get; set; }

        // Profit margin range
        [Display(Name = "Min Profit Margin")]
        public decimal? MinProfitMargin { get; set; }

        [Display(Name = "Max Profit Margin")]
        public decimal? MaxProfitMargin { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        // Employee filter for Report F
        [Display(Name = "Employee")]
        public string? EmployeeId { get; set; }

        // Review status filter for Report F
        [Display(Name = "Status")]
        public string ReviewStatus { get; set; } // "approved", "rejected", "all"
    }
}
