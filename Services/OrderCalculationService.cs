using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace fa25group23final.Services
{
    public class OrderCalculationService
    {
        private readonly AppDbContext _context;

        public OrderCalculationService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calculate all order totals - ONE PLACE for all calculation logic
        /// </summary>
        public async Task<OrderTotals> CalculateTotals(Orders order)
        {
            // Get current shipping config
            var shippingConfig = await _context.ShippingConfigs
                .FirstOrDefaultAsync(s => s.IsActive);

            if (shippingConfig == null)
            {
                shippingConfig = new Shippingconfig
                {
                    FirstBookFee = 3.50m,
                    AdditionalBookFee = 1.50m,
                    IsActive = true,
                    UpdatedBy = "System"
                };
            }

            // Calculate subtotal and item count from OrderDetails
            decimal subtotal = order.OrderDetails.Sum(od => od.Price * od.Quantity);
            int itemCount = order.OrderDetails.Sum(od => od.Quantity);

            // Calculate shipping fee
            decimal shippingFee = itemCount == 1
                ? shippingConfig.FirstBookFee
                : shippingConfig.FirstBookFee + ((itemCount - 1) * shippingConfig.AdditionalBookFee);

            // Calculate discount amount
            decimal discountAmount = 0m;
            if (order.couponID != null)
            {
                var coupon = order.couponID;

                if (coupon.CouponType == CouponTypeEnum.Discount && coupon.DiscountPercent.HasValue)
                {
                    discountAmount = subtotal * (coupon.DiscountPercent.Value / 100m);
                }
                else if (coupon.CouponType == CouponTypeEnum.FreeShipping)
                {
                    if (!coupon.FreeThreshold.HasValue || subtotal >= coupon.FreeThreshold.Value)
                    {
                        discountAmount = shippingFee;
                    }
                }
            }

            // Calculate total
            decimal total = subtotal + shippingFee - discountAmount;
            if (total < 0) total = 0;

            return new OrderTotals
            {
                Subtotal = subtotal,
                ItemCount = itemCount,
                ShippingFee = shippingFee,
                DiscountAmount = discountAmount,
                Total = total
            };
        }

        /// <summary>
        /// Recalculate weighted average cost for a book after receiving new inventory.
        /// Call this when books arrive at loading dock.
        /// </summary>
        /// param: bookId = The book ID
        /// param: newCost = Cost per unit for newly received books
        /// param: newQuantity = Quantity of books just received
        /// returns: New weighted average cost
        public async Task<decimal> RecalculateBookCost(int bookId, decimal newCost, int newQuantity)
        {
            // Get all previously received reorders for this book (excluding current receive)
            var previousReorders = await _context.ReorderDetails
                .Where(rd => rd.Book.BookID == bookId && rd.QuantityReceived > 0)
                .ToListAsync();

            // Calculate total cost and quantity from history
            decimal totalCost = previousReorders.Sum(rd => rd.Cost * rd.QuantityReceived);
            int totalQty = previousReorders.Sum(rd => rd.QuantityReceived);

            // Add newly received (will be saved after this method returns)
            totalCost += newCost * newQuantity;
            totalQty += newQuantity;

            if (totalQty == 0) return newCost;

            return totalCost / totalQty;
        }

        /// <summary>
        /// Get weighted average profit margin for a book.
        /// Profit margin = (weighted avg selling price - weighted avg cost) / weighted avg selling price * 100
        /// </summary>
        /// <param name="bookId">The book ID</param>
        /// <returns>Profit margin percentage, or null if no data</returns>
        public async Task<decimal?> GetProfitMargin(int bookId)
        {
            // Weighted avg price from completed customer sales
            var sales = await _context.OrderDetails
                .Where(od => od.bookID.BookID == bookId && od.orderID.OrderStatus == true)
                .ToListAsync();

            if (!sales.Any()) return null;

            decimal totalRevenue = sales.Sum(od => od.Price * od.Quantity);
            int totalSold = sales.Sum(od => od.Quantity);
            decimal avgPrice = totalRevenue / totalSold;

            // Weighted avg cost from received reorders
            var reorders = await _context.ReorderDetails
                .Where(rd => rd.Book.BookID == bookId && rd.QuantityReceived > 0)
                .ToListAsync();

            if (!reorders.Any())
            {
                // Fall back to current BookCost if no reorder history
                var book = await _context.Books.FindAsync(bookId);
                if (book == null || book.BookCost == 0) return null;

                return (avgPrice - book.BookCost) / avgPrice * 100;
            }

            decimal totalCost = reorders.Sum(rd => rd.Cost * rd.QuantityReceived);
            int totalReceived = reorders.Sum(rd => rd.QuantityReceived);
            decimal avgCost = totalCost / totalReceived;

            if (avgPrice == 0) return null;

            return (avgPrice - avgCost) / avgPrice * 100;
        }

        /// <summary>
        /// Get report totals for all orders (Report D)
        /// </summary>
        /// <returns>Total revenue, cost, and profit</returns>
        public async Task<ReportTotals> GetReportTotals()
        {
            // All completed customer order details
            var orderDetails = await _context.OrderDetails
                .Where(od => od.orderID.OrderStatus == true)
                .ToListAsync();

            decimal totalRevenue = orderDetails.Sum(od => od.Price * od.Quantity);
            decimal totalCost = orderDetails.Sum(od => od.Cost * od.Quantity);
            decimal totalProfit = totalRevenue - totalCost;

            return new ReportTotals
            {
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                TotalProfit = totalProfit
            };
        }

        /// <summary>
        /// Get total inventory value (Report E)
        /// </summary>
        /// <returns>Sum of (inventory quantity * weighted avg cost) for all books</returns>
        public async Task<decimal> GetInventoryValue()
        {
            var books = await _context.Books
                .Where(b => b.InventoryQuantity > 0)
                .ToListAsync();

            return books.Sum(b => b.InventoryQuantity * b.BookCost);
        }
    }

    /// Return object for order calculations
    public class OrderTotals
    {
        public decimal Subtotal { get; set; }
        public int ItemCount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
    }

    /// Return object for report totals (Report D)
    public class ReportTotals
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalProfit { get; set; }
    }
}
