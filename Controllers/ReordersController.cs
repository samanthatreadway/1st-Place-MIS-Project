using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace fa25group23final.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReordersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OrderCalculationService _calculationService;

        public ReordersController(AppDbContext context, OrderCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        // GET: Reorder/Index
        // Loading dock view - shows all pending reorder items to receive
        public async Task<IActionResult> Index()
        {
            var pendingItems = await _context.ReorderDetails
                .Include(rd => rd.Book)
                .Include(rd => rd.Reorder)
                .Where(rd => rd.QuantityOrdered > rd.QuantityReceived)
                .Where(rd => rd.Reorder.OrderDate != null) // only placed orders
                .OrderBy(rd => rd.Reorder.OrderDate)
                .ToListAsync();

            return View(pendingItems);
        }

        // POST: Reorder/Receive
        // Check in books at loading dock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receive(int reorderDetailsId, int quantityArrived)
        {
            var rd = await _context.ReorderDetails
                .Include(r => r.Book)
                .Include(r => r.Reorder)
                .FirstOrDefaultAsync(r => r.ReorderDetailsID == reorderDetailsId);

            if (rd == null)
            {
                TempData["ErrorMessage"] = "Order item not found.";
                return RedirectToAction(nameof(Index));
            }

            if (quantityArrived < 0)
            {
                TempData["ErrorMessage"] = "Quantity received cannot be negative.";
                return RedirectToAction(nameof(Index));
            }

            // Cap at remaining quantity (per spec: don't accept excess)
            int maxAllowed = rd.QuantityOrdered - rd.QuantityReceived;
            int actualReceived = Math.Min(quantityArrived, maxAllowed);

            if (actualReceived == 0)
            {
                TempData["InfoMessage"] = "No additional books to receive for this item.";
                return RedirectToAction(nameof(Index));
            }

            // Update ReorderDetails
            rd.QuantityReceived += actualReceived;

            // Update Book inventory
            rd.Book.InventoryQuantity += actualReceived;

            // Recalculate weighted average cost
            rd.Book.BookCost = await _calculationService.RecalculateBookCost(rd.Book.BookID, rd.Cost, actualReceived);

            await _context.SaveChangesAsync();

            string message = $"Received {actualReceived} copies of '{rd.Book.Title}'.";
            if (quantityArrived > maxAllowed)
            {
                message += $" ({quantityArrived - actualReceived} excess copies returned to supplier.)";
            }

            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        // ============================================
        // MANUAL ORDER - MULTI-TITLE (CART-LIKE FLOW)
        // ============================================

        // GET: Reorder/ManualOrder
        // Shows book search and current draft reorder
        public async Task<IActionResult> ManualOrder()
        {
            return RedirectToAction("ViewDraft");
        }

            // GET: Reorder/ViewDraft
            // Shows current draft order
        public async Task<IActionResult> ViewDraft()
        {
            var draft = await GetOrCreateDraftReorder();
            var draftItems = await _context.ReorderDetails
                .Include(rd => rd.Book)
                .Where(rd => rd.Reorder.ReorderID == draft.ReorderID)
                .ToListAsync();

            return View(draftItems);
            // Book search
            //var booksQuery = _context.Books.Where(b => b.BookStatus); // active books only

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    booksQuery = booksQuery.Where(b =>
            //        b.Title.Contains(searchString) ||
            //        b.Authors.Contains(searchString) ||
            //        b.BookNumber.ToString().Contains(searchString));
            //    ViewBag.SearchString = searchString;
            //}

            //var books = await booksQuery.OrderBy(b => b.Title).Take(20).ToListAsync();

            //return View(books);
        }

        // POST: Reorder/AddToDraft
        // Add a book to the draft reorder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToDraft(int bookId, int quantity, decimal cost)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            if (cost <= 0)
            {
                TempData["ErrorMessage"] = "Cost must be greater than zero.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                TempData["ErrorMessage"] = "Book not found.";
                return RedirectToAction("Details", "Books", new { id = bookId });
            }

            var draft = await GetOrCreateDraftReorder();

            // Check if book already in draft
            var existingItem = await _context.ReorderDetails
                .FirstOrDefaultAsync(rd => rd.Reorder.ReorderID == draft.ReorderID && rd.Book.BookID == bookId);

            if (existingItem != null)
            {
                // Update existing item
                existingItem.QuantityOrdered += quantity;
                existingItem.Cost = cost;
                TempData["SuccessMessage"] = $"Updated '{book.Title}' in draft order. Total qty: {existingItem.QuantityOrdered}";
            }
            else
            {
                // Add new item
                var reorderDetail = new ReorderDetails
                {
                    Reorder = draft,
                    Book = book,
                    QuantityOrdered = quantity,
                    Cost = cost,
                    QuantityReceived = 0
                };
                _context.ReorderDetails.Add(reorderDetail);
                TempData["SuccessMessage"] = $"Added '{book.Title}' to draft order.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ViewDraft");
        }

        // POST: Reorder/UpdateDraftItem
        // Update quantity/cost of item in draft
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDraftItem(int reorderDetailsId, int quantity, decimal cost)
        {
            var item = await _context.ReorderDetails
                .Include(rd => rd.Book)
                .Include(rd => rd.Reorder)
                .FirstOrDefaultAsync(rd => rd.ReorderDetailsID == reorderDetailsId);

            if (item == null || item.Reorder.OrderDate != null)
            {
                TempData["ErrorMessage"] = "Item not found or order already placed.";
                return RedirectToAction(nameof(ViewDraft));
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction(nameof(ViewDraft));
            }

            if (cost <= 0)
            {
                TempData["ErrorMessage"] = "Cost must be greater than zero.";
                return RedirectToAction(nameof(ViewDraft));
            }

            item.QuantityOrdered = quantity;
            item.Cost = cost;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Updated '{item.Book.Title}'.";
            return RedirectToAction(nameof(ViewDraft));
        }

        // POST: Reorder/RemoveFromDraft
        // Remove a book from draft reorder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromDraft(int reorderDetailsId)
        {
            var item = await _context.ReorderDetails
                .Include(rd => rd.Book)
                .Include(rd => rd.Reorder)
                .FirstOrDefaultAsync(rd => rd.ReorderDetailsID == reorderDetailsId);

            if (item == null || item.Reorder.OrderDate != null)
            {
                TempData["ErrorMessage"] = "Item not found or order already placed.";
                return RedirectToAction(nameof(ViewDraft));
            }

            string bookTitle = item.Book.Title;
            _context.ReorderDetails.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Removed '{bookTitle}' from draft order.";
            return RedirectToAction(nameof(ViewDraft));
        }

        // POST: Reorder/PlaceDraftOrder
        // Finalize and place the draft order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceDraftOrder()
        {
            var draft = await _context.Reorders
                .Include(r => r.ReorderDetails)
                .FirstOrDefaultAsync(r => r.OrderDate == null);

            if (draft == null || !draft.ReorderDetails.Any())
            {
                TempData["ErrorMessage"] = "No items in draft order.";
                return RedirectToAction(nameof(ViewDraft));
            }

            // Place the order by setting OrderDate
            draft.OrderDate = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully placed order for {draft.ReorderDetails.Count} book(s).";
            return RedirectToAction(nameof(Index));
        }

        // ============================================
        // AUTO REORDER - BOOKS BELOW REORDER POINT
        // ============================================

        // GET: Reorder/AutoReorder
        // Shows books below reorder point (accounting for pending orders)
        public async Task<IActionResult> AutoReorder()
        {
            var booksToReorder = await GetBooksNeedingReorder();

            // Build list with default values
            var reorderList = booksToReorder.Select(b => new ReorderDetails
            {
                Book = b,
                QuantityOrdered = 5, // default per spec
                Cost = b.BookCost > 0 ? b.BookCost : 0.01m // default to last cost
            }).ToList();

            return View(reorderList);
        }

        // POST: Reorder/PlaceAutoOrder
        // Bulk create reorders from auto-reorder list
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceAutoOrder(List<int> bookIds, List<int> quantities, List<decimal> costs)
        {
            if (bookIds == null || !bookIds.Any())
            {
                TempData["ErrorMessage"] = "No items to order.";
                return RedirectToAction(nameof(AutoReorder));
            }

            // Build valid items list
            var validItems = new List<(int bookId, int qty, decimal cost)>();

            for (int i = 0; i < bookIds.Count; i++)
            {
                int qty = quantities != null && i < quantities.Count ? quantities[i] : 0;
                decimal cost = costs != null && i < costs.Count ? costs[i] : 0;

                if (qty > 0 && cost > 0)
                {
                    validItems.Add((bookIds[i], qty, cost));
                }
            }

            if (!validItems.Any())
            {
                TempData["ErrorMessage"] = "No valid items to order. Ensure quantity > 0 and cost > 0.";
                return RedirectToAction(nameof(AutoReorder));
            }

            // Create a new Reorder
            var reorder = new Reorders
            {
                OrderDate = DateTime.Now
            };

            _context.Reorders.Add(reorder);
            await _context.SaveChangesAsync();

            // Add each item to the reorder
            foreach (var (bookId, qty, cost) in validItems)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null) continue;

                var reorderDetail = new ReorderDetails
                {
                    Reorder = reorder,
                    Book = book,
                    QuantityOrdered = qty,
                    Cost = cost,
                    QuantityReceived = 0
                };

                _context.ReorderDetails.Add(reorderDetail);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully placed order for {validItems.Count} book(s).";
            return RedirectToAction(nameof(Index));
        }

        // ============================================
        // HISTORY
        // ============================================

        // GET: Reorder/History
        // View all past reorders (completed)
        public async Task<IActionResult> History()
        {
            var completedOrders = await _context.Reorders
                .Include(r => r.ReorderDetails)
                    .ThenInclude(rd => rd.Book)
                .Where(r => r.OrderDate != null)
                .OrderByDescending(r => r.OrderDate)
                .ToListAsync();

            return View(completedOrders);
        }

        // ============================================
        // HELPER METHODS
        // ============================================

        // Get or create a draft reorder (OrderDate = null)
        private async Task<Reorders> GetOrCreateDraftReorder()
        {
            var draft = await _context.Reorders
                .FirstOrDefaultAsync(r => r.OrderDate == null);

            if (draft == null)
            {
                draft = new Reorders { OrderDate = null };
                _context.Reorders.Add(draft);
                await _context.SaveChangesAsync();
            }

            return draft;
        }

        // Get books that need reordering (below reorder point, accounting for pending)
        private async Task<List<Books>> GetBooksNeedingReorder()
        {
            var activeBooks = await _context.Books
                .Where(b => b.BookStatus) // active books only
                .ToListAsync();

            var booksNeedingReorder = new List<Books>();

            foreach (var book in activeBooks)
            {
                // Calculate pending quantity (ordered but not yet received)
                var pendingQty = await _context.ReorderDetails
                    .Where(rd => rd.Book.BookID == book.BookID)
                    .Where(rd => rd.Reorder.OrderDate != null) // only placed orders
                    .SumAsync(rd => rd.QuantityOrdered - rd.QuantityReceived);

                // Check if below reorder point (inventory + pending < reorder point)
                if (book.InventoryQuantity + pendingQty < book.ReorderPoint)
                {
                    booksNeedingReorder.Add(book);
                }
            }

            return booksNeedingReorder;
        }
    }
}
