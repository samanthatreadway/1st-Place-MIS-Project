using fa25group23final.DAL;
using fa25group23final.Models;
using fa25group23final.Services;
using fa25group23final.Utilities;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace fa25group23final.Controllers
{
    
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ReportService _reportService;

        public BooksController(AppDbContext context, ReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        [AllowAnonymous]
        public IActionResult Index(string SearchString, string? SortOrder, bool? stock)
        {
            ViewData["CurrentSearch"] = SearchString;
            ViewData["CurrentSort"] = SortOrder;

            var query = from bk in _context.Books
                        .Include(b => b.orderDetails)
                        .Include(r => r.reviews.Where(r => r.DisputeStatus == true))
                        .Include(g => g.genre)
                        select bk;


            // Apply search filter FIRST (while still IQueryable)
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(bk => bk.Title.Contains(SearchString) ||
                    bk.BookDescription.Contains(SearchString) ||
                    bk.Authors.Contains(SearchString) || bk.BookNumber.ToString().Contains(SearchString));
            }

            if (stock == true)
            {
                query = query.Where(b => b.InventoryQuantity >= 1);

            }


            // Materialize to memory BEFORE sorting by reviews
            List<Books> SelectedBooks = query.ToList();

            // Now sort in memory where filtered reviews are already loaded
            if (SortOrder != null)
            {
                SelectedBooks = SortBooks(SelectedBooks, SortOrder);
            }

            ViewBag.BookCountTotal = _context.Books.Count();
            ViewBag.BookCountSelected = SelectedBooks.Count();


            ViewBag.CurrentSearchString = SearchString;
            ViewBag.CurrentSortOrder = SortOrder;

            return View(SelectedBooks);
            
        }

        private List<Books> SortBooks(List<Books> books, string sortOrder)
        {
            return sortOrder switch
            {
                "title_asc" => books.OrderBy(b => b.Title).ToList(),
                "title_desc" => books.OrderByDescending(b => b.Title).ToList(),

                "author_asc" => books.OrderBy(b => b.Authors).ToList(),
                "author_desc" => books.OrderByDescending(b => b.Authors).ToList(),

                "newest" => books.OrderByDescending(b => b.PublishDate).ToList(),
                "oldest" => books.OrderBy(b => b.PublishDate).ToList(),

                // SAFE version — handles books with no reviews
                "highest_rated" => books
                    .OrderByDescending(b => b.AverageRating).ToList(),

                //"most_popular" => books
                //.OrderByDescending(b =>b.orderDetails?.Sum(od => od.Quantity) ?? 0).ToList(),
                "most_popular" => books
                    .OrderByDescending(b =>
                        (b.orderDetails?.Sum(od => od.Quantity)) ?? 0
                    )
                    .ToList(),

                _ => books.OrderBy(b => b.Title).ToList(), // default
            };
        }

        private SelectList GetAllGenresSelectList()
        {
            //get list of genres from database
            List<Genres> genresList = _context.Genres.ToList();

            //add dummy select all
            Genres SelectNone = new Genres() { GenreID = 0, GenreName = "All Genres" };
            genresList.Add(SelectNone);

            //convert list
            SelectList genreSelectList = new SelectList(genresList.OrderBy(m => m.GenreID), "GenreID", "GenreName");

            //return select List
            return genreSelectList;


        }

        [AllowAnonymous]
        public IActionResult DetailedSearch()
        {
            ViewBag.AllGenres = GetAllGenresSelectList();

            return View();
        }

        [AllowAnonymous]
        public IActionResult DisplaySearchResults(SearchViewModel svm)
        {

            if ((svm.userRating != null && svm.Rating == null) ||
                (svm.userRating == null && svm.Rating != null))
            {
                ModelState.AddModelError("", "If you enter rating, you must select comparison");
                ViewBag.AllGenres = GetAllGenresSelectList();
                return View("DetailedSearch", svm);
            }

            var query = from bk in _context.Books
                        .Include(r => r.reviews.Where(r => r.DisputeStatus == true))
                        .Include(g => g.genre)
                        select bk;

            foreach (var b in query)
            {
                var avg = b.AverageRating;
            }



            if (svm.Title != null) //if title is inputted
            {
                query = query.Where(b => b.Title.Contains(svm.Title));
            }

            if (svm.Desc != null) //if descriptions is inputted
            {
                query = query.Where(b => b.BookDescription.Contains(svm.Desc));
            }

            if (svm.Author != null) //if author is inputted
            {
                query = query.Where(b => b.Authors.Contains(svm.Author));
            }

            if (svm.Genre != 0) // if not 0 the genre selected is not All
            {
                query = query.Where(b => b.genre.GenreID == svm.Genre);
            }

            if (svm.Date != null)
            {
                query = query.Where(b => b.PublishDate >= svm.Date);
            }

            if (svm.Price != null)
            {
                //greater than
                if (svm.priceComparison == PriceComparisonEnum.greaterThan)
                {
                    query = query.Where(b => b.BookPrice >= svm.Price);
                }
                else if (svm.priceComparison == PriceComparisonEnum.lessThan)
                {
                    query = query.Where(b => b.BookPrice <= svm.Price);
                }
                else
                {
                    query = query.Where(b => b.BookPrice == svm.Price);
                }
            }

            if (svm.Number != null)
            {
                query = query.Where(b => b.BookNumber == svm.Number);
            }

            if (svm.Stock == true)
            {
                query = query.Where(b => b.InventoryQuantity >= 1);
            }


            if (svm.userRating != null)
            {
                //Query to include all the books that have at least 1 active review
                query = query.Where(b => b.reviews.Any(r => r.DisputeStatus == true));

                if (svm.Rating == RatingComparisonEnum.greaterThan)
                {

                    //query = query.Where(b => b.AverageRating > svm.userRating);
                    query = query.Where(b => b.reviews.Count() > 0 &&
                                     (decimal)b.reviews.Average(r => r.Rating) >= svm.userRating);

                }
                else if (svm.Rating == RatingComparisonEnum.lessThan)
                {
                    //query = query.Where(b => b.AverageRating < svm.userRating);
                    query = query.Where(b => b.reviews.Count() > 0 &&
                                           (decimal)b.reviews.Average(r => r.Rating) <= svm.userRating);

                }
                //else
                //{
                //    //query = query.Where(b => b.AverageRating == svm.userRating);
                //    query = query.Where(b => b.reviews.Count() > 0 &&
                //                           b.reviews.Average(r => r.Rating) == (double)svm.userRating);
                //}
            }


            //add and create list object to send back to index method

            List<Books> selectedBooks = query.ToList();

            ViewBag.BookCountTotal = _context.Books.Count();
            ViewBag.BookCountSelected = selectedBooks.Count();

            return View("Index", selectedBooks);
        }




        // GET: Books/Details/5
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .Include(b => b.genre)
                .Include(b => b.reviews.Where(r => r.DisputeStatus == true)) // if you want ratings loaded
                .FirstOrDefaultAsync(m => m.BookID == id);

            if (books == null)
            {
                return NotFound();
            }

            // ---------- ALREADY IN CART CHECK ----------
            bool alreadyInCart = false;

            if (User.Identity.IsAuthenticated && User.IsInRole("Customer"))
            {
                // Find the customer's open cart
                var openOrder = await _context.Orders
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.bookID)
                    .Include(o => o.customerID)
                    .FirstOrDefaultAsync(o =>
                        o.OrderStatus == false &&                // open cart
                        o.customerID.UserName == User.Identity.Name);

                if (openOrder != null)
                {
                    alreadyInCart = openOrder.OrderDetails
                        .Any(od => od.bookID.BookID == books.BookID);
                }
            }

            ViewBag.AlreadyInCart = alreadyInCart;
            // ------------------------------------------

            // ---------- ADMIN: AVERAGE PROFIT MARGIN ----------
            if (User.IsInRole("Admin"))
            {
                decimal? averageProfitMargin = await _reportService.GetAverageProfitMarginPerBook(books.BookID);
                ViewBag.AverageProfitMargin = averageProfitMargin;
            }
            // --------------------------------------------------

            return View(books);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.AllGenres = GetAllGenresSelectList()
                .Where(g => g.Value != "0");   // Removes All Genres

            return View();
        }


        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Books book, int? SelectedGenreID, string NewGenreName)
        {
            Genres genreToUse = null;

            // USER SELECTED EXISTING GENRE
            if (SelectedGenreID.HasValue && SelectedGenreID.Value != 0)
            {
                genreToUse = await _context.Genres.FindAsync(SelectedGenreID.Value);
            }
            // USER ENTERED NEW GENRE
            else if (!string.IsNullOrWhiteSpace(NewGenreName))
            {
                genreToUse = new Genres { GenreName = NewGenreName };
                _context.Genres.Add(genreToUse);
                await _context.SaveChangesAsync(); // Needed to generate GenreID
            }
            // NEITHER SELECTED NOR CREATED
            else
            {
                ModelState.AddModelError("", "You must select or create a genre.");
                ViewBag.AllGenres = GetAllGenresSelectList()
                    .Where(g => g.Value != "0");   // Removes All Genres
                return View(book);
            }


            // Attach FK
            book.genre = genreToUse;

            // Attach unique book number
            book.BookNumber = Utilities.GenerateNextBookNumber.GetNextBookNumber(_context);

            _context.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }




        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var books = await _context.Books
                .Include(b => b.genre)                  // Include the related Genre
               .FirstOrDefaultAsync(b => b.BookID == id); // Fetch the book by ID


            if (books == null)
            {
                return NotFound();
            }

            ViewBag.AllGenres = GetAllGenresSelectList()
                .Where(g => g.Value != "0");   // Removes All Genres

            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Books books, int? SelectedGenreID, string? NewGenreName)
        {
            //TODO
            if (books == null) throw new Exception("Books model is null!");


            if (id != books.BookID)
            {
                return NotFound();
            }

            var oldBook = await _context.Books
                                .AsNoTracking()
                                .FirstOrDefaultAsync(b => b.BookID == books.BookID);

            bool isNowDiscontinued = false;

            // BookStatus changed from true → false
            if (oldBook != null && oldBook.BookStatus == true && books.BookStatus == false)
            {
                isNowDiscontinued = true;
            }


            if (ModelState.IsValid)
            {

                try
                {
                    var bk = await _context.Books
                        .Include(b => b.genre)
                        .FirstOrDefaultAsync(b => b.BookID == books.BookID);

                    bk.Title = books.Title;
                    bk.Authors = books.Authors;
                    bk.BookDescription = books.BookDescription;
                    bk.BookPrice = books.BookPrice;
                    bk.BookCost = books.BookCost;
                    bk.PublishDate = books.PublishDate;
                    bk.ReorderPoint = books.ReorderPoint;
                    bk.BookStatus = books.BookStatus;

                    //TODO
                    if (bk == null) return NotFound();

                    // HANDLE GENRE
                    Genres genreToUse = null;

                    if (SelectedGenreID.HasValue && SelectedGenreID.Value != 0)
                    {
                        genreToUse = await _context.Genres.FindAsync(SelectedGenreID.Value);
                    }
                    else if (!string.IsNullOrWhiteSpace(NewGenreName))
                    {
                        genreToUse = new Genres { GenreName = NewGenreName };
                        _context.Genres.Add(genreToUse);
                        await _context.SaveChangesAsync(); // Generate GenreID
                    }
                    else
                    {
                        ModelState.AddModelError("", "You must select or create a genre.");
                        ViewBag.AllGenres = GetAllGenresSelectList()
                            .Where(g => g.Value != "0");   // Removes All Genres
                        return View(books);
                    }

                    // Assign GenreID to book
                    bk.genre = genreToUse;

                    await _context.SaveChangesAsync();


                    // Handle discontinued book cart emails
                    if (isNowDiscontinued)
                    {
                        // Find all open carts (OrderStatus == false) containing this book
                        var carts = await _context.Orders
                            .Include(o => o.customerID)
                            .Include(o => o.OrderDetails)
                                .ThenInclude(od => od.bookID)
                            .Where(o => o.OrderStatus == false &&
                                        o.OrderDetails.Any(od => od.bookID.BookID == books.BookID))
                            .ToListAsync();

                        foreach (var order in carts)
                        {
                            var od = order.OrderDetails.First(od => od.bookID.BookID == books.BookID);

                            // Build customer-specific email
                            string subject = $"Book Discontinued: {books.Title}";
                            string body =
                                $"A book in a your shopping cart was just discontinued.\n\n" +
                                $"Book: {books.Title}\n" +
                                $"Author(s): {books.Authors}\n\n" +
                                $"Affected Customer:\n" +
                                $"{order.customerID.FirstName} {order.customerID.LastName}\n" +
                                $"Email: {order.customerID.Email}\n" +
                                $"Quantity in cart: {od.Quantity}\n\n" +
                                $"The book will be automatically removed the next time you view your cart.";

                            // Send one email PER customer/cart
                            EmailMessaging.SendEmail(subject, body);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksExists(books.BookID))
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

            ViewBag.AllGenres = GetAllGenresSelectList()
                            .Where(g => g.Value != "0");   // Removes All Genres

            return View(books);
        }

        // GET: Books/Delete/5
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var books = await _context.Books
        //        .FirstOrDefaultAsync(m => m.BookID == id);
        //    if (books == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(books);
        //}

        //// POST: Books/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var books = await _context.Books.FindAsync(id);
        //    if (books != null)
        //    {
        //        _context.Books.Remove(books);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.BookID == id);
        }
    }
}
