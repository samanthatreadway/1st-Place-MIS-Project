using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Models.ViewModels.Orders;
using fa25group23final.Services;
using fa25group23final.Utilities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace fa25group23final.__useDefaultLayout
{
    // TODO: Uncomment authorization when ready
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly OrderCalculationService _calculationService;  // Inject
        private readonly RecommendationService _recommendationService;


        public OrdersController(AppDbContext context, UserManager<AppUser> userManager, OrderCalculationService calculationService, RecommendationService recommendationService)
        {
            _context = context;
            _userManager = userManager;
            _calculationService = calculationService;
            _recommendationService = recommendationService;
        }


        // GET: Orders
        [Authorize(Roles = ("Admin, Employee"))]
        public async Task<IActionResult> EmployeeIndex()
        {
            List<Orders> Orders = new List<Orders>();


            // see all orders (customer & admin) that are completed
            Orders = _context.Orders
                .Where(o => o.OrderStatus)
                .Include(ord => ord.OrderDetails)
                    .ThenInclude(od => od.bookID)
                .Include(ord => ord.customerID)
                .Include(ord => ord.couponID)
                .Include(ord => ord.creditCard)    
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(Orders);
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {
            List<Orders> Orders = new List<Orders>();
            if (User.IsInRole("Customer"))
            {
                Orders = _context.Orders
                    .Where(o => o.customerID.UserName == User.Identity.Name && o.OrderStatus)
                    .Include(ord => ord.OrderDetails)
                    .ThenInclude(od => od.bookID)
                    .Include(ord => ord.couponID)
                    .Include(ord => ord.creditCard)      

                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                // see all customer orders that are completed
                Orders = _context.Orders
                    .Where(o => o.OrderStatus)
                    .Include(ord => ord.OrderDetails)
                    .ThenInclude(od => od.bookID)
                    .Include(ord => ord.customerID)
                    .Include(ord => ord.couponID)
                    .Include(ord => ord.creditCard)   
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }

                return View(Orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Pleaes specify an order to view!" });
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.bookID)
                        .ThenInclude(b => b.genre)
                .Include(o => o.customerID)
                .Include(o => o.creditCard)
                .Include(o => o.couponID)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return View("Error", new String[] { "This order was not found!" });
            }

            if (User.IsInRole("Customer") && order.customerID.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "You are not authorized to view this order!" });
            }

            // ===================== CALCULATE TOTALS (EXISTING) =====================
            var totals = await _calculationService.CalculateTotals(order);

            ViewBag.Subtotal = totals.Subtotal;
            ViewBag.ItemCount = totals.ItemCount;
            ViewBag.ShippingFee = totals.ShippingFee;
            ViewBag.DiscountAmount = totals.DiscountAmount;
            ViewBag.Total = totals.Total;

            // Format credit card display
            if (order.creditCard != null)
            {
                var cardNumber = order.creditCard.CardNumber;
                string last4 = (!string.IsNullOrEmpty(cardNumber) && cardNumber.Length >= 4)
                    ? cardNumber.Substring(cardNumber.Length - 4)
                    : cardNumber ?? "****";

                string masked = "************" + last4;
                ViewBag.CreditCardDisplay = $"{order.creditCard.CardType} {masked}";
            }
            else
            {
                ViewBag.CreditCardDisplay = "Not yet selected";
            }


            // ===================== BOOK RECOMMENDATIONS =====================

            // AppUser Id for this order's customer (used inside RecommendationService)
            string userId = order.customerID.Id;

            // Pick ONE book from this order to base the recommendations on
            int? baseBookId = order.OrderDetails
                                   .Select(od => (int?)od.bookID.BookID)
                                   .FirstOrDefault();

            List<Books> recs = new List<Books>();

            if (baseBookId.HasValue)
            {
                recs = _recommendationService
                    .GetRecommendationsForBook(baseBookId.Value, userId);
            }

            // Pass list of recommended books to the view via ViewBag
            ViewBag.RecommendedBooks = recs;

            // ================================================================

            return View(order);
        }

        // GET: Orders/ShoppingCart
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> ShoppingCart()
        {
            // Get the customer's shopping cart
            var cart = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.bookID)
                .FirstOrDefaultAsync(o => o.customerID.UserName == User.Identity.Name && o.OrderStatus == false);

            if (cart == null)
            {
                TempData["InfoMessage"] = "Your cart is empty.";
                var emptyVm = new ShoppingCartViewModel();
                return View(emptyVm);
                //return View("Error", new string[] { "Unable to load shopping cart" });
            }

            var shippingConfig = await _context.ShippingConfigs.FirstOrDefaultAsync(s => s.IsActive);
            ViewData["ShippingConfig"] = shippingConfig;

            // Build the view model
            var viewModel = await BuildShoppingCartViewModel(cart);

            return View(viewModel);
        }

        // POST: Orders/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> UpdateQuantity(int orderDetailsId, int quantity)
        {
            if (quantity < 1)
            {
                TempData["ErrorMessage"] = "Quantity must be at least 1";
                return RedirectToAction(nameof(ShoppingCart));
            }

            // Find the order detail
            var orderDetail = await _context.OrderDetails
                .Include(od => od.bookID)
                .Include(od => od.orderID)
                    .ThenInclude(o => o.customerID)
                .FirstOrDefaultAsync(od => od.OrderDetailsID == orderDetailsId);

            if (orderDetail == null)
            {
                TempData["ErrorMessage"] = "Item not found in cart";
                return RedirectToAction(nameof(ShoppingCart));
            }

            // Verify this belongs to the current user's cart
            if (orderDetail.orderID.customerID.UserName != User.Identity.Name ||
                orderDetail.orderID.OrderStatus == true)
            {
                return View("Error", new string[] { "Invalid cart item" });
            }

            // Check stock availability
            if (quantity > orderDetail.bookID.InventoryQuantity)
            {
                TempData["ErrorMessage"] = $"Cannot add {quantity} items. Only {orderDetail.bookID.InventoryQuantity} in stock.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            // Update quantity
            orderDetail.Quantity = quantity;

            // Update price to reflect current price
            orderDetail.Price = orderDetail.bookID.BookPrice;
            orderDetail.Cost = orderDetail.bookID.BookCost;

            _context.Update(orderDetail);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Quantity updated successfully";
            return RedirectToAction(nameof(ShoppingCart));
        }

        // GET: Orders/Create
        //TODO: not sure if this should be customer authorization only
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Create()
        {
            //if (User.IsInRole("Customer"))
            //{
                Orders ord = new Orders();
                ord.customerID = await _userManager.FindByNameAsync(User.Identity.Name);
                return View(ord);
            //}
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Create([Bind("OrderID,OrderDate,ShippingFee,OrderStatus")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // orders.OrderDate = DateTime.Now;

            orders.customerID = await _userManager.FindByNameAsync(orders.customerID.UserName);

            _context.Add(orders);
            await _context.SaveChangesAsync();

            return RedirectToAction("Create", "OrderDetails", new {orderID = orders.OrderID});
        }


        // customer add to cart
        [Authorize(Roles = ("Customer"))]
        public IActionResult AddToCart(int? BookID)
        {
            if (BookID == null)
            {
                return View("Error", new string[] { "Please specify a book to add to your shopping cart" });
            }

            //find the book in the database
            Books dbBook = _context.Books.Find(BookID);

            //make sure the book exists in the database
            if (dbBook == null)
            {
                return View("Error", new string[] { "This book was not in our inventory!" });
            }

            // Check if book is discontinued
            if (!dbBook.BookStatus)
            {
                TempData["ErrorMessage"] = "This book has been discontinued and cannot be added to cart.";
                return RedirectToAction("Details", "Books", new { id = BookID });
            }

            // Check if book is in stock
            if (dbBook.InventoryQuantity <= 0)
            {
                TempData["ErrorMessage"] = "This book is currently out of stock.";
                return RedirectToAction("Details", "Books", new { id = BookID });
            }

            //find the cart for this customer
            Orders ord = _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.bookID)
                .FirstOrDefault(o => o.customerID.UserName == User.Identity.Name && o.OrderStatus == false);

            //if this order is null, there isn't one yet, so create it
            if (ord == null)
            {
                //create a new object
                ord = new Orders();

                ord.OrderStatus = false;
                ord.customerID = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                ord.OrderDate = null;
                //add the order to the database
                _context.Orders.Add(ord);
                _context.SaveChanges();
            }

            var existingOrderDetail = ord.OrderDetails.FirstOrDefault(od => od.bookID.BookID == BookID);

            if (existingOrderDetail != null)
            {
                if (existingOrderDetail.Quantity + 1 > dbBook.InventoryQuantity)
                {
                    TempData["ErrorMessage"] = $"Cannot add more. Only {dbBook.InventoryQuantity} in stock.";
                    return RedirectToAction("Details", "Books", new { id = BookID });
                }

                existingOrderDetail.Quantity += 1;
                existingOrderDetail.Price = dbBook.BookPrice;
                existingOrderDetail.Cost = dbBook.BookCost;

                _context.Update(existingOrderDetail);
                _context.SaveChanges();
            }
            else
            {
                //now create the order detail
                OrderDetails od = new OrderDetails();

                //add the book to the order detail
                od.bookID = dbBook;
                //add the order to the order detail
                od.orderID = ord;
                od.Quantity = 1;
                od.Price = dbBook.BookPrice;
                od.Cost = dbBook.BookCost;
                // od.TotalPrice = dbBook.BookPrice * od.Quantity;

                //add the order detail to the database
                _context.OrderDetails.Add(od);
                _context.SaveChanges(true);
            }
            TempData["SuccessMessage"] = $"{dbBook.Title} added to cart!";
            //go to the details view
            // return RedirectToAction("Details", new { id = ord.OrderID });
            return RedirectToAction(nameof(ShoppingCart));
        }


        // GET: Orders/Edit/5
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order to edit" });
            }
            
            //TODO: what is this error....
            Orders orders = _context.Orders
                         .Include(o => o.OrderDetails)
                         .ThenInclude(o => o.bookID)
                         .Include(o => o.customerID)
                         .FirstOrDefault(o => o.OrderID == id);
            
            if (orders == null)
            {
                return View("Error", new String[] { "This order was not found in the database!" });
            }

            if (User.IsInRole("Customer") && orders.customerID.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "You are not authorized to edit this registration!" });
            }

            //registration is complete - cannot be edited
            if (orders.OrderStatus == true)
            {
                return View("Error", new string[] { "This order is complete and cannot be changed!" });
            }

            return View(orders);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderDate,ShippingFee,OrderStatus")] Orders orders)
        {
            if (id != orders.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // POST: Orders/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> RemoveFromCart(int orderDetailsId)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.orderID)
                .ThenInclude(o => o.customerID)
                .FirstOrDefaultAsync(od => od.OrderDetailsID == orderDetailsId);

            if (orderDetail == null)
            {
                TempData["ErrorMessage"] = "Item not found in cart";
                return RedirectToAction(nameof(ShoppingCart));
            }

            // TODO: change this for admin later
            // Verify this belongs to the current user's cart
            if (orderDetail.orderID.customerID.UserName != User.Identity.Name ||
                orderDetail.orderID.OrderStatus == true)
            {
                return View("Error", new string[] { "Invalid cart item" });
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Item removed from cart";
            return RedirectToAction(nameof(ShoppingCart));
        }


        // GET: Orders/Checkout
        // Purpose: Display checkout page with current cart, credit cards, and any applied coupon
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Checkout()
        {
            var cart = await GetUserCart();

            if (cart == null || !cart.OrderDetails.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add items before checking out.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            var viewModel = await BuildCheckoutViewModel(cart);
            return View(viewModel);
        }

        // POST: Orders/ApplyCoupon
        // Purpose: Apply coupon code to cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var cart = await GetUserCart();

            if (cart == null || !cart.OrderDetails.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            var (isValid, message, coupon) = await ValidateCoupon(couponCode, cart);

            if (isValid && coupon != null)
            {
                cart.couponID = coupon;
                _context.Update(cart);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Checkout));
        }

        // POST: Orders/RemoveCoupon
        // Purpose: Remove applied coupon from cart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> RemoveCoupon()
        {
            var cart = await GetUserCart();

            if (cart != null && cart.couponID != null)
            {
                cart.couponID = null;
                _context.Update(cart);
                await _context.SaveChangesAsync();
                TempData["InfoMessage"] = "Coupon removed from order.";
            }

            return RedirectToAction(nameof(Checkout));
        }

        // POST: Orders/PlaceOrder
        // Purpose: Complete the order - validate & apply coupon, calculate totals,
        //          reduce inventory, finalize order
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> PlaceOrder(int selectedCreditCardId)
        {
            var cart = await GetUserCart();

            // Validation 1: Cart exists and has items
            if (cart == null || !cart.OrderDetails.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(ShoppingCart));
            }

            // Validation 2: Credit card belongs to user
            var creditCard = await _context.CreditCards
                .Include(cc => cc.CustomerID)
                .FirstOrDefaultAsync(cc => cc.CardID == selectedCreditCardId &&
                                           cc.CustomerID.UserName == User.Identity.Name);

            if (creditCard == null)
            {
                TempData["ErrorMessage"] = "Invalid credit card selected.";
                return RedirectToAction(nameof(Checkout));
            }

            // Calculate totals using services
            var totals = await _calculationService.CalculateTotals(cart);

            // Update order
            cart.OrderDate = DateTime.Now;
            cart.OrderStatus = true;
            cart.ShippingFee = totals.ShippingFee;
            cart.creditCard = creditCard;
            cart.ShippingAddress = cart.customerID.Address;

            // Reduce inventory
            foreach (var orderDetail in cart.OrderDetails)
            {
                // bookID here is a navigation property to Books
                var book = orderDetail.bookID;
                book.InventoryQuantity -= orderDetail.Quantity;
                _context.Books.Update(book);
            }

            _context.Orders.Update(cart);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Order placed successfully!";

            // ============== RECOMMENDATIONS FOR EMAIL ==============

            string userId = _userManager.GetUserId(User);

            // Pick ONE book from the order to base recommendations on (first line item)
            int? baseBookId = cart.OrderDetails
                                  .Select(od => (int?)od.bookID.BookID)
                                  .FirstOrDefault();

            List<Books> recommendedBooks = new List<Books>();

            if (baseBookId.HasValue)
            {
                recommendedBooks = _recommendationService
                    .GetRecommendationsForBook(baseBookId.Value, userId);
            }

            // ===================== EMAIL CONFIRMATION =====================
            
            try
            {
                string subject = $"Order Confirmation - Order #{cart.OrderID}";

                string body = $"Thank you for your order from Bevo's Books!\n\n" +
                              $"Order Details:\n" +
                              $"Order ID: {cart.OrderID}\n" +
                              $"Order Date: {cart.OrderDate}\n" +
                              $"Customer: {cart.customerID.FirstName} {cart.customerID.LastName}\n" +
                              $"Email: {cart.customerID.Email}\n" +
                              $"Shipping Address: {cart.ShippingAddress}\n\n" +
                              $"Card Charged: {creditCard.CardType} ************{creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4)}\n\n" +
                              $"Items:\n";

                foreach (var od in cart.OrderDetails)
                {
                    body += $"- {od.bookID.Title} (Qty: {od.Quantity}) @ {od.Price:C}\n";
                }

                body += $"\nOrder Summary:\n" +
                        $"Subtotal: {totals.Subtotal:C}\n" +
                        $"Shipping Fee: {totals.ShippingFee:C}\n";
                if (totals.DiscountAmount > 0)
                {
                    body += $"Discount: -{totals.DiscountAmount:C}\n";
                }
                body += $"Total: {totals.Total:C}\n";

                if (recommendedBooks.Any())
                {
                    body += "\n==========================================\n";
                    body += "You Might Also Like:\n";
                    body += "==========================================\n";
                    foreach (var b in recommendedBooks)
                    {
                        body += $"- {b.Title} by {b.Authors}\n";
                    }
                }

                EmailMessaging.SendEmail(subject, body);
            }
            catch (Exception e)
            {
                Console.WriteLine("Email sending failed: " + e.Message);
            }
            

            // Redirect to the order details "confirmation" page
            return RedirectToAction(nameof(Details), new { id = cart.OrderID });
        }


        [Authorize(Roles = ("Customer"))]
        public async Task<IActionResult> Confirm(int? id)
        {
            Orders dbOrd = await _context.Orders.FindAsync(id);
            dbOrd.OrderStatus = true;
            _context.Update(dbOrd);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orders = await _context.Orders
        //        .FirstOrDefaultAsync(m => m.OrderID == id);
        //    if (orders == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orders);
        //}

        //// POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var orders = await _context.Orders.FindAsync(id);
        //    if (orders != null && orders.OrderStatus == false)
        //    {
        //        _context.Orders.Remove(orders);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // ==================== HELPER METHODS ====================

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }


        // <summary>
        /// Builds shopping cart view model with validation and automatic updates
        /// - Updates all prices to current values
        /// - Removes out-of-stock items (InventoryQuantity = 0)
        /// - Removes discontinued items (BookStatus = false)
        /// - Shows one-time messages using session
        /// - Adjusts quantities that exceed available stock
        /// </summary>
        private async Task<ShoppingCartViewModel> BuildShoppingCartViewModel(Orders cart)
        {
            var viewModel = new ShoppingCartViewModel
            {
                Messages = new List<string>()
            };

            // Track items to remove and session key for one-time messages
            var itemsToRemove = new List<OrderDetails>();
            var sessionKey = "RemovedItems_" + cart.OrderID;

            // Get list of items already shown in messages (for one-time display)
            var shownRemovedItems = HttpContext.Session.GetString(sessionKey);
            var shownItemIds = string.IsNullOrEmpty(shownRemovedItems)
                ? new HashSet<int>()
                : new HashSet<int>(shownRemovedItems.Split(',').Select(int.Parse));

            var newlyRemovedItems = new List<int>();

            foreach (var orderDetail in cart.OrderDetails.ToList())
            {
                var book = orderDetail.bookID;
                bool shouldRemove = false;
                string removalReason = "";

                // Check if book is discontinued (BookStatus = false)
                if (!book.BookStatus)
                {
                    shouldRemove = true;
                    removalReason = $"'{book.Title}' has been discontinued and was removed from your cart.";
                }
                // Check if book is out of stock (InventoryQuantity = 0)
                else if (book.InventoryQuantity <= 0)
                {
                    shouldRemove = true;
                    removalReason = $"'{book.Title}' is out of stock and was removed from your cart.";
                }

                if (shouldRemove)
                {
                    itemsToRemove.Add(orderDetail);

                    // Only show message if we haven't shown it before (one-time display)
                    if (!shownItemIds.Contains(orderDetail.OrderDetailsID))
                    {
                        viewModel.Messages.Add(removalReason);
                        newlyRemovedItems.Add(orderDetail.OrderDetailsID);
                    }
                    continue;
                }

                // Update price to current price (always show current prices)
                var priceChanged = false;
                if (orderDetail.Price != book.BookPrice)
                {
                    orderDetail.Price = book.BookPrice;
                    orderDetail.Cost = book.BookCost;
                    priceChanged = true;
                }

                // Check if quantity exceeds available stock
                if (orderDetail.Quantity > book.InventoryQuantity)
                {
                    orderDetail.Quantity = book.InventoryQuantity;
                    viewModel.Messages.Add($"The quantity for '{book.Title}' was adjusted to {book.InventoryQuantity} (maximum available stock).");
                }

                // Notify user of price changes
                if (priceChanged)
                {
                    viewModel.Messages.Add($"The price for '{book.Title}' has been updated to {book.BookPrice:C}.");
                }
            }

            // Remove discontinued and out-of-stock items from database
            if (itemsToRemove.Any())
            {
                // removes multiple items at once from orderDetails
                _context.OrderDetails.RemoveRange(itemsToRemove);
                await _context.SaveChangesAsync();

                // Update session with newly removed items (for one-time message display)
                if (newlyRemovedItems.Any())
                {
                    shownItemIds.UnionWith(newlyRemovedItems);
                    HttpContext.Session.SetString(sessionKey, string.Join(",", shownItemIds));
                }
            }
            else
            {
                // Save any price or quantity updates
                await _context.SaveChangesAsync();
            }

            // Reload cart items after removals
            await _context.Entry(cart)
                .Collection(o => o.OrderDetails)
                .Query()
                .Include(od => od.bookID)
                .LoadAsync();

            // Set the items in the view model
            viewModel.Items = cart.OrderDetails.ToList();

            return viewModel;
        }

        /// Build checkout view model
        private async Task<CheckoutViewModel> BuildCheckoutViewModel(Orders cart)
        {
            var user = await _userManager.GetUserAsync(User);
            var creditCards = await _context.CreditCards
                .Where(cc => cc.CustomerID.Id == user.Id)
                .ToListAsync();

            var viewModel = new CheckoutViewModel
            {
                Items = cart.OrderDetails.ToList(),
                Messages = new List<string>(),
                AppliedCoupon = cart.couponID,
                CouponCode = cart.couponID?.CouponCode,
                ShippingConfig = await GetShippingConfig(),
                ShippingAddress = user.Address,

                // TODO in the future: When you separate address fields in AppUser, uncomment these:
                // Street = user.Street,
                // City = user.City,
                // State = user.State,
                // ZipCode = user.ZipCode,
            };

            viewModel.CreditCardOptions = creditCards.Select(cc => new SelectListItem
            {
                Value = cc.CardID.ToString(),
                Text = $"{cc.CardType} ************{cc.CardNumber.Substring(cc.CardNumber.Length - 4)}"
            }).ToList();

            // Calculate totals for display
            // viewModel.CalculateCheckoutTotals();
            var totals = await _calculationService.CalculateTotals(cart);
            viewModel.ShippingFee = totals.ShippingFee;
            viewModel.DiscountAmount = totals.DiscountAmount;
            viewModel.Total = totals.Total;

            return viewModel;
        }


        /// Gets the active shipping configuration from database
        /// Creates default config if none exists
        private async Task<Shippingconfig> GetShippingConfig()
        {
            var config = await _context.ShippingConfigs
                .Where(sc => sc.IsActive)
                .FirstOrDefaultAsync();

            // If no config exists, create default one
            if (config == null)
            {
                config = new Shippingconfig
                {
                    FirstBookFee = 3.50m,
                    AdditionalBookFee = 1.50m,
                    IsActive = true,
                    UpdatedBy = "System"
                };

                _context.ShippingConfigs.Add(config);
                await _context.SaveChangesAsync();
            }

            return config;
        }


        /// Get user's cart - simple helper to avoid repeating this query
        private async Task<Orders> GetUserCart()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.bookID)
                .Include(o => o.customerID)
                .Include(o => o.couponID)
                .FirstOrDefaultAsync(o => o.customerID.UserName == User.Identity.Name &&
                                          o.OrderStatus == false);
        }

        /// <summary>
        /// Validate coupon code
        /// </summary>
        private async Task<(bool isValid, string message, Coupon coupon)> ValidateCoupon(string? couponCode,Orders cart)
        {
            if (string.IsNullOrWhiteSpace(couponCode))
            {
                return (false, "Please enter a coupon code.", null);
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.CouponCode.ToUpper() == couponCode.ToUpper());

            if (coupon == null)
            {
                return (false, $"Coupon code '{couponCode}' does not exist.", null);
            }

            if (!coupon.Status)
            {
                return (false, $"Coupon code '{couponCode}' is no longer active.", null);
            }

            // Check if customer already used this coupon
            var alreadyUsed = await _context.Orders
                .Include(o => o.couponID)
                .AnyAsync(o => o.customerID.UserName == User.Identity.Name &&
                              o.OrderStatus == true &&
                              o.couponID.CouponID == coupon.CouponID);

            if (alreadyUsed)
            {
                return (false, $"You have already used coupon code '{couponCode}'.", null);
            }

            // Validate based on coupon type
            if (coupon.CouponType == CouponTypeEnum.FreeShipping)
            {
                if (coupon.FreeThreshold.HasValue && coupon.FreeThreshold.Value > 0)
                {
                    decimal subtotal = cart.OrderDetails.Sum(od => od.Price * od.Quantity);

                    if (subtotal < coupon.FreeThreshold.Value)
                    {
                        return (false,
                               $"Order subtotal must be at least {coupon.FreeThreshold.Value:C} to use this coupon. Current subtotal: {subtotal:C}",
                               null);
                    }

                    return (true, $"Free shipping coupon applied! (Minimum order: {coupon.FreeThreshold.Value:C})", coupon);
                }

                return (true, "Free shipping coupon applied!", coupon);
            }
            else if (coupon.CouponType == CouponTypeEnum.Discount)
            {
                if (coupon.DiscountPercent.HasValue)
                {
                    return (true, $"Discount coupon applied: {coupon.DiscountPercent}% off your order!", coupon);
                }

                return (false, "This discount coupon is not properly configured.", null);
            }

            return (false, "Invalid coupon type.", null);
        }

    }
}
