using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace fa25group23final.__useDefaultLayout
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReviewsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Admin and Employee see ALL reviews
            if (User.IsInRole("Admin") || User.IsInRole("Employee"))
            {
                var allReviews = await _context.Reviews
                    .Include(r => r.reviewer)
                    .Include(r => r.approver)
                    .Include(r => r.bookID)        // <-- REQUIRED
                    .ToListAsync();

                return View(allReviews);
            }

            // Customer sees only THEIR reviews
            var user = await _userManager.GetUserAsync(User);

            var myReviews = await _context.Reviews
                .Include(r => r.reviewer)
                .Include(r => r.approver)
                .Include(r => r.bookID)            // <-- REQUIRED
                .Where(r => r.reviewerID == user.Id)
                .ToListAsync();

            return View(myReviews);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AllReviews()
        {
           
            var allReviews = await _context.Reviews
                .Include(r => r.reviewer)
                .Include(r => r.bookID)        // <-- REQUIRED
                .ToListAsync();

            return View(allReviews);
            
        }


        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.approver)
                .Include(r => r.reviewer)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (reviews == null)
            {
                return NotFound();
            }

            //var bookName = _context.Books.FirstOrDefault(b => b.BookID == reviews.bookID.BookID).Title;
            //ViewBag.BookName = bookName;

            return View(reviews);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Book ID invalid" });
            }

            if (_context.Books.FirstOrDefault(b => b.BookNumber == id) == null)
            {
                //the bookID inputted is not real in database
                return View("Error", new string[] { "Book is not Found" });
            }


            //else the book is real !

            ViewBag.bookID = id;

            var userID = _userManager.GetUserId(User);
            ViewBag.userID = userID;

            var bookName = _context.Books.FirstOrDefault(b => b.BookNumber == id).Title;
            ViewBag.BookName = bookName;


            //Check if the customer purchased the book

            var dbBook = _context.Books.FirstOrDefault(b => b.BookNumber == id);
            var user = _context.Users.FirstOrDefault(u => u.Id == userID);

            //find the users confirmed ordres
            var order = _context.Orders
                .Where(m => m.customerID == user && m.OrderStatus == true)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.bookID)
                .ToList();


            //check if the user already wrote a review
            var rv = _context.Reviews.FirstOrDefault(m => m.bookID.BookNumber == id && m.reviewer == user);
            if (rv != null)
            {
                return View("Error", new string[] { "You have already written a review for this book" });
            }


            //now check if the book matches
            if (order.Count() <= 0)
            {
                return View("Error", new string[] { "You have not purchased this book!" });
            }


            foreach( Orders o in order)
            {
                foreach(OrderDetails od in o.OrderDetails)
                {
                    var bk = od.bookID;

                    if (bk == dbBook)
                    {
                        return View();
                    }

                }
            }

            return View("Error", new string[] { "You have not purchased this book!" });
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reviews r, int book)
        {

            //get the book object
            Books bookObject = _context.Books.FirstOrDefault(b => b.BookNumber == book);

            //get user data to connec to review object
            var userID = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userID);

            if (user == null)
            {
                return View("Error", new string[] { "User is Null" });
            }

            try
            {
                //create new review object
                Reviews review = new Reviews
                {
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    reviewer = user,
                    reviewerID = userID,
                    bookID = bookObject
                };

                _context.Reviews.Add(review);
                _context.SaveChanges();

                //send user to success view
                return View("ReviewSuccess");

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "Failed to Create a Review" });
            }
        }

        // GET: Reviews/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null) //validate ID is not null
        //    {
        //        return View("Error", new string[] { "Review ID is null" });
        //    }

        //    //validate the review id exists



        //    var reviews = await _context.Reviews.FindAsync(id);
        //    if (reviews == null) //validate if review is real
        //    {
        //        return View("Error", new string[] { "Could not Find Review to Edit" });
        //    }

        //    //if this far review exists in DB, check if the users review, if not error
        //    var user = await _userManager.GetUserAsync(User);

        //    if (reviews.reviewer.Id != user.Id)
        //    {
        //        return View("Error", new string[] { "Review you are trying to Access is unauthorized" });
        //    }

        //    //if this far, send review object to change

        //    ViewBag.userID = user.Id;
        //    ViewBag.bookID = _context.Books.FirstOrDefault(b => b.BookID == id);


        //    return View(reviews);
        //}

        //// POST: Reviews/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, Reviews reviews)
        //{
        //    if (id != reviews.ReviewID)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        Reviews dbReview = _context.Reviews.FirstOrDefault(r => r.ReviewID == reviews.ReviewID);

        //        dbReview.Rating = reviews.Rating;
        //        dbReview.ReviewText = reviews.ReviewText;
        //        dbReview.DisputeStatus = null;
            
        //        _context.Reviews.Update(dbReview);
        //        _context.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error", new string[] { "There was an error updating this Review" });

        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.approver)
                .Include(r => r.reviewer)
                .FirstOrDefaultAsync(m => m.ReviewID == id);
            if (reviews == null)
            {
                return NotFound();
            }


            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reviews = await _context.Reviews.FindAsync(id);
            if (reviews != null)
            {
                _context.Reviews.Remove(reviews);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewID == id);
        }


        [Authorize(Roles = "Admin, Employee")]
        public IActionResult ApproveReviews()
        {
            var dbReviews = _context.Reviews
                                .Include(u => u.reviewer)
                                .ToList();

            return View(dbReviews);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult UpdateReview(Reviews rv)
        {
            try
            {
                Reviews dbReview = _context.Reviews.FirstOrDefault(r => r.ReviewID == rv.ReviewID);

                dbReview.DisputeStatus = rv.DisputeStatus;

                _context.Reviews.Update(dbReview);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an Error Approving the Review" });

            }
            
            return RedirectToAction(nameof(ApproveReviews));
            
        }

        [AllowAnonymous]
        public IActionResult BooksReviews(int? id)
        {
            //find all reviews related to the book
            var dbReviews = _context.Reviews
                .Include(m => m.reviewer)
                .Include(m => m.bookID)
                .Where(m => m.DisputeStatus == true)
                .Where(m => m.bookID.BookID == id).ToList();

            if (dbReviews != null)
            {
                return View(dbReviews);
            }

            return View("Error", new string[] { "There are no reviews for this book" });

        }

        public async Task<IActionResult> ToReview()
        {
            var user = await _userManager.GetUserAsync(User);

            //get list of books user has purchased
            var purchasedBooks = _context.Orders
                .Where(o => o.customerID.Id == user.Id)
                .SelectMany(o => o.OrderDetails)
                .Select(od => od.bookID)
                .Distinct()
                .ToList();

            var reviews = _context.Reviews
                .Include(b => b.bookID)
                .Where(m => m.reviewer.Id == user.Id)
                .Select(r => r.bookID.BookID)
                .ToList();

            var booksToReview = purchasedBooks
                .Where(b => !reviews.Contains(b.BookID))
                .ToList();

            return View(booksToReview);
        }

    }
}
