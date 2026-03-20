using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;

namespace fa25group23final.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public IActionResult Index(int? orderID)
        {
            if(orderID == null) {   
                return View("Error", new String[] {"Please specify an order to view details for."});
            }

            List<OrderDetails> ods = _context.OrderDetails
                .Include(od => od.bookID)
                .Where(od => od.orderID.OrderID == orderID)
                .ToList();
            return View(ods);
        }

        // GET: OrderDetails/Create
        public IActionResult Create(int orderID)
        {
            OrderDetails od = new OrderDetails();
            Orders dbOrder = _context.Orders.Find(orderID);

            od.orderID = dbOrder;
            return View(od);
        }

        // POST: OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailsID,Price,Cost,Quantity")] OrderDetails orderDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderDetails);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetails = await _context.OrderDetails.FindAsync(id);
            if (orderDetails == null)
            {
                return NotFound();
            }

            //TODO: add the email functionality 

            return View(orderDetails);
        }

        // POST: OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailsID,Price,Cost,Quantity")] OrderDetails orderDetails)
        {
            if (id != orderDetails.OrderDetailsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailsExists(orderDetails.OrderDetailsID))
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
            return View(orderDetails);
        }

        //// GET: OrderDetails/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetails = await _context.OrderDetails
        //        .FirstOrDefaultAsync(m => m.OrderDetailsID == id);
        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderDetails);
        //}

        //// POST: OrderDetails/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var orderDetails = await _context.OrderDetails.FindAsync(id);
        //    if (orderDetails != null)
        //    {
        //        _context.OrderDetails.Remove(orderDetails);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderDetailsExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailsID == id);
        }
    }
}
