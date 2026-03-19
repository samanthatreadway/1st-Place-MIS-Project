using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


using fa25group23final.Models;
using fa25group23final.DAL;
using System;

namespace fa25group23final.Controllers
{
    public class SeedController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedController(AppDbContext db, UserManager<AppUser> um, RoleManager<IdentityRole> rm)
        {
            _context = db;
            _userManager = um;
            _roleManager = rm;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> SeedRoles()
        {
            try
            {
                //call the method to seed the roles
                await Seeding.SeedRoles.AddAllRoles(_roleManager);
            }
            catch (Exception ex)
            {
                //add the error messages to a list of strings
                List<String> errorList = new List<String>();

                //Add the outer message
                errorList.Add(ex.Message);

                //Add the message from the inner exception
                errorList.Add(ex.InnerException.Message);

                //Add additional inner exception messages, if there are any
                if (ex.InnerException.InnerException != null)
                {
                    errorList.Add(ex.InnerException.InnerException.Message);
                }

                return View("Error", errorList);
            }

            //this is the happy path - seeding worked!
            return View("Confirm");
        }
        public async Task<IActionResult> SeedPeople()
        {
            try
            {
                //call the method to seed the users
                await Seeding.SeedUsers.SeedAllUsers(_userManager, _context);
            }
            catch (Exception ex)
            {
                //add the error messages to a list of strings
                List<String> errorList = new List<String>();

                //Add the outer message
                errorList.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    //Add the message from the inner exception
                    errorList.Add(ex.InnerException.Message);

                    //Add additional inner exception messages, if there are any
                    if (ex.InnerException.InnerException != null)
                    {
                        errorList.Add(ex.InnerException.InnerException.Message);
                    }

                }


                return View("Error", errorList);
            }

            //this is the happy path - seeding worked!
            return View("Confirm");
        }

        // DONE: seeded genres
        public async Task<IActionResult> SeedGenres()
        {
            try
            {
                Seeding.SeedGenres.SeedAllGenres(_context);
            }
            catch (Exception ex)
            {
                List<String> errorList = new List<String>();
                errorList.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    errorList.Add(ex.InnerException.Message);

                    if (ex.InnerException.InnerException != null)
                    {
                        errorList.Add(ex.InnerException.InnerException.Message);
                    }
                }

                return View("Error", errorList);
            }

            return View("Confirm");
        }

        // DONE: seeded credit cards
        public async Task<IActionResult> SeedCards()
        {
            try
            {
                Seeding.SeedCreditCards.SeedAllCreditCards(_context);
            }
            catch (Exception ex)
            {
                List<String> errorList = new List<String>();
                errorList.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    errorList.Add(ex.InnerException.Message);

                    if (ex.InnerException.InnerException != null)
                    {
                        errorList.Add(ex.InnerException.InnerException.Message);
                    }
                }

                return View("Error", errorList);
            }

            return View("Confirm");
        }

        // DONE: seeded books, orders, order details, reviews
        public async Task<IActionResult> SeedAll()
        {
            try
            {
                Seeding.SeedBooks.SeedAllBooks(_context);
                Seeding.SeedOrders.SeedAllOrders(_context);
                Seeding.SeedOrderDetails.SeedAllOrderDetails(_context);
                Seeding.SeedReviews.SeedAllReviews(_context);
            }
            catch (Exception ex)
            {
                List<String> errorList = new List<String>();
                errorList.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    errorList.Add(ex.InnerException.Message);

                    if (ex.InnerException.InnerException != null)
                    {
                        errorList.Add(ex.InnerException.InnerException.Message);
                    }
                }

                return View("Error", errorList);
            }

            return View("Confirm");
        }

    }
}
