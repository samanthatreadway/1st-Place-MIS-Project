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

namespace fa25group23final.Controllers
{
    //TODO add Authorize for Admins only
    //[Authorize(Roles = "Admin")]
    public class CouponController : Controller
    {
        private readonly AppDbContext _context;

        public CouponController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Coupon
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }

        // GET: Coupon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Coupon ID is empty" });
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponID == id);

            if (coupon == null)
            {
                return View("Error", new string[] { "Coupon does not exist" });
            }

            return View(coupon);
        }

        // GET: Coupon/Create
        public IActionResult Create()
        {
            return View(); //return create view
        }

        public IActionResult CreateFinal(Coupon coupon)
        {
            //We receive two attributes from the Create View:
            //Coupon Code, and Coupon Type

           
            bool exists = _context.Coupons.Any(c => c.CouponCode == coupon.CouponCode);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Coupon Code already Exists");
                return View("Create", coupon);

            }

            //validate inputs
            if (coupon.CouponCode == null || coupon.CouponType == null)
            {
                return View("Create", coupon);
            }


            //Validate the coupon code is unqiue
            //TRY/CATCH etc Return View("Create");


            //Coupon Code is Unique, therefore Valid
            //Check if coupon type is Free Shipping OR X% Off
            if (coupon.CouponType == CouponTypeEnum.Discount) // type is discount
            {
                return View("CreateDiscount", coupon);
            }
            else
            {
                //type is shipping
                return View("CreateShipping", coupon);
            }
            
        }


        // POST: Coupon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            
            if (coupon.CouponType == CouponTypeEnum.Discount)
            {
                //coupon type is discount, validate inputs
                if (coupon.DiscountPercent == null)
                {
                    return View("CreateDiscount", coupon); // send back to view because invalid input
                }
            }
            else //else Coupon type is Free Shipping
            {
                if (coupon.FreeThreshold == null)
                {
                    return View("CreateShipping", coupon); // send back to view
                }
            }

            //if past this code, then data shoudl be validated, and add to the DB

            _context.Add(coupon);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Coupon/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Coupon id is empty" });
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return View("Error", new string[] { "Coupon does not exist" });

            }
            return View(coupon);
        }

        // POST: Coupon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Coupon coupon)
        {
            if (id != coupon.CouponID)
            {
                return View("Error", new string[] { "ID Does not Match" });

            }

            try
            {
                _context.Update(coupon);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(coupon.CouponID))
                {
                    return View("Error", new string[] { "Update Failed" });
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Coupon/Delete/5
        //TODO: Delete Delete methods after the coupon system works (comment out)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(m => m.CouponID == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        // POST: Coupon/Delete/5
        //TODO: Delete (comment out)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.CouponID == id);
        }
    }
}
