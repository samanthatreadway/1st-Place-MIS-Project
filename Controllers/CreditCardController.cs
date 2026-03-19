using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace fa25group23final.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CreditCardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CreditCardController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CreditCard
        public async Task<IActionResult> Index()
        {
            //Send only current logged in users card to index view
            var user = await _userManager.GetUserAsync(User); // Get user object

            var userCard = _context.CreditCards // grab from DB context only the logged in user
                .Where(c => c.CustomerID == user)
                .ToList();

            return View(userCard); //return only list of user object to view

        }

        // GET: CreditCard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            
            if (id == null)
            {
                return View("Error", new string[] { "Credit Card Does not Exist" });
            }

            //VALIDATE THAT CARD IS USERS CARD NOT OTHER
            var user = await _userManager.GetUserAsync(User); // get AppUser object

            List<CreditCard> creditCardList = _context.CreditCards // get list of credit cards that match the logged in users
                .Where(m => m.CustomerID == user)
                .ToList();

            //delcare a bool that says if the ID inputted is logged in users
            bool usersIdBool = false;

            // find list of all credit card ids that the user has
            foreach ( CreditCard credCard in creditCardList)
            {
                if (credCard.CardID == id) //if inputted id matches any of the CardIDs that user has linked
                {
                    usersIdBool = true; // allow access to view details of the card
                }
            }

            if (!usersIdBool) // if false, then user tyring to access other person details
            {
                return View("Error", new string[] { "Unauthorized Access: User ID if Not Yours" });
            }

            //Otherwise add credit card details to list
            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(m => m.CardID == id);

            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // GET: CreditCard/Create
        public IActionResult Create()
        {
            //Get the UserID to send into the view
            var userID = _userManager.GetUserId(User);
            ViewBag.ID = userID;

            return View();
        }

        // POST: CreditCard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CardNumber,CardType")] CreditCard creditCard, string ID)
        {
            //intakes ID hidden value from view to find the current logged in user
            var user = await _userManager.FindByIdAsync(ID);

            // 2. Count how many cards this user ALREADY has in the DB
            int userCardLength = await _context.CreditCards
                .Where(c => c.CustomerID.Id == user.Id)   // or .Where(c => c.CustomerID == user)
                .CountAsync();

            // 3. Enforce 0–3 rule (block the 4th card)
            if (userCardLength >= 3)
            {
                // you can also use ModelState.AddModelError instead of Error view if you prefer
                return View("Error", new string[] { "Max amount of credit cards (3) has been reached." });
            }

            //Else create new card
            creditCard.CustomerID = user;
            user.AppUserCreditCardList.Add(creditCard);



            _context.Add(creditCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }

        // GET: CreditCard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Credit Card Does not Exist" });
            }

            //VALIDATE THAT CARD IS USERS CARD NOT OTHER
            var user = await _userManager.GetUserAsync(User); // get AppUser object

            List<CreditCard> creditCardList = _context.CreditCards // get list of credit cards that match the logged in users
                .Where(m => m.CustomerID == user)
                .ToList();

            //delcare a bool that says if the ID inputted is logged in users
            bool usersIdBool = false;

            // find list of all credit card ids that the user has
            foreach (CreditCard credCard in creditCardList)
            {
                if (credCard.CardID == id) //if inputted id matches any of the CardIDs that user has linked
                {
                    usersIdBool = true; // allow access to view details of the card
                }
            }

            if (!usersIdBool) // if false, then user tyring to access other person details
            {
                return View("Error", new string[] { "Unauthorized Access: User ID if Not Yours" });
            }

            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // POST: CreditCard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreditCard creditCard)
        {
            //if (id != creditCard.CardID)
            //{
            //    return View("Error", new string[] { "Please try again" });

            //}

            if (creditCard.CardNumber == null || creditCard.CardType == null)
            {
                return View(creditCard);
            }

            try
            {
                CreditCard dbCard = _context.CreditCards
                    .FirstOrDefault(q => q.CardID == creditCard.CardID);

                dbCard.CardNumber = creditCard.CardNumber;
                dbCard.CardType = creditCard.CardType;

                _context.CreditCards.Update(dbCard);
                _context.SaveChanges();

            }
            catch
            {
                return View("Error", new string[] { "There was an error editing this card" });

            }

            return RedirectToAction(nameof(Index));


        }

        // GET: CreditCard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditCard = await _context.CreditCards
                .FirstOrDefaultAsync(m => m.CardID == id);
            if (creditCard == null)
            {
                return NotFound();
            }

            return View(creditCard);
        }

        // POST: CreditCard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard != null)
            {
                _context.CreditCards.Remove(creditCard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditCardExists(int id)
        {
            return _context.CreditCards.Any(e => e.CardID == id);
        }
    }
}
