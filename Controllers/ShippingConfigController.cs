using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using fa25group23final.DAL;
using fa25group23final.Models;

namespace fa25group23final.Controllers
{
    [Authorize(Roles = "Admin")]  // Uncomment when roles are configured
    public class ShippingConfigController : Controller
    {
        private readonly AppDbContext _context;

        public ShippingConfigController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ShippingConfig
        public async Task<IActionResult> Index()
        {
            var configs = await _context.ShippingConfigs.ToListAsync();

            return View(configs);
        }

       /* // GET: ShippingConfig/Details/5 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingConfig = await _context.ShippingConfigs
                .FirstOrDefaultAsync(m => m.ShippingConfigID == id);

            if (shippingConfig == null)
            {
                return NotFound();
            }

            return View(shippingConfig);
        } */

        // GET: ShippingConfig/Create
        public IActionResult Create()
        {
            var model = new Shippingconfig
            {
                FirstBookFee = 3.50m,
                AdditionalBookFee = 1.50m,
                IsActive = true
            };
            return View(model);
        } 

        // POST: ShippingConfig/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Shippingconfig shippingConfig)
        {
            // Make sure UpdatedBy and IsActive are set on the server
            if (string.IsNullOrEmpty(shippingConfig.UpdatedBy))
            {
                shippingConfig.UpdatedBy = User.Identity?.Name ?? "System";
            }

            // If you always want new ones active, enforce it here too
            shippingConfig.IsActive = true;

            if (!ModelState.IsValid)
            {
                // TEMP: log why ModelState is invalid
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"MODEL ERROR - Key: {entry.Key} | Error: {error.ErrorMessage}");
                    }
                }

                return View(shippingConfig);
            }

            // If this is set to active, deactivate all others
            if (shippingConfig.IsActive)
            {
                var activeConfigs = await _context.ShippingConfigs
                    .Where(sc => sc.IsActive)
                    .ToListAsync();

                foreach (var config in activeConfigs)
                {
                    config.IsActive = false;
                }
            }

            _context.Add(shippingConfig);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Shipping configuration created successfully";
            return RedirectToAction(nameof(Index));
        }

        // GET: ShippingConfig/Edit/5
        /*   public async Task<IActionResult> Edit(int? id)
           {
               if (id == null)
               {
                   return NotFound();
               }

               var shippingConfig = await _context.ShippingConfigs.FindAsync(id);
               if (shippingConfig == null)
               {
                   return NotFound();
               }
               return View(shippingConfig);
           }

           // POST: ShippingConfig/Edit/5
           [HttpPost]
           [ValidateAntiForgeryToken]
           public async Task<IActionResult> Edit(int id, [Bind("ShippingConfigID,FirstBookFee,AdditionalBookFee,IsActive")] Shippingconfig shippingConfig)
           {
               if (id != shippingConfig.ShippingConfigID)
               {
                   return NotFound();
               }

               if (ModelState.IsValid)
               {
                   try
                   {
                       // If this is set to active, deactivate all others
                       if (shippingConfig.IsActive)
                       {
                           var activeConfigs = await _context.ShippingConfigs
                               .Where(sc => sc.IsActive && sc.ShippingConfigID != id)
                               .ToListAsync();

                           foreach (var config in activeConfigs)
                           {
                               config.IsActive = false;
                           }
                       }
                       shippingConfig.UpdatedBy = User.Identity.Name;

                       _context.Update(shippingConfig);
                       await _context.SaveChangesAsync();

                       TempData["SuccessMessage"] = "Shipping configuration updated successfully";
                   }
                   catch (DbUpdateConcurrencyException)
                   {
                       if (!ShippingConfigExists(shippingConfig.ShippingConfigID))
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
               return View(shippingConfig);
           } */

        // GET: ShippingConfig/SetActive/5
        public async Task<IActionResult> SetActive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingConfig = await _context.ShippingConfigs.FindAsync(id);
            if (shippingConfig == null)
            {
                return NotFound();
            }

            // Deactivate all others
            var activeConfigs = await _context.ShippingConfigs
                .Where(sc => sc.IsActive)
                .ToListAsync();

            foreach (var config in activeConfigs)
            {
                config.IsActive = false;
            }

            // Activate this one
            shippingConfig.IsActive = true;
            shippingConfig.UpdatedBy = User.Identity.Name;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Active shipping configuration updated";
            return RedirectToAction(nameof(Index));
        }
    }
}
        // GET: ShippingConfig/Delete/5
     /*   public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shippingConfig = await _context.ShippingConfigs
                .FirstOrDefaultAsync(m => m.ShippingConfigID == id);

            if (shippingConfig == null)
            {
                return NotFound();
            }

            return View(shippingConfig);
        }

        // POST: ShippingConfig/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shippingConfig = await _context.ShippingConfigs.FindAsync(id);
            
            if (shippingConfig != null)
            {
                // Don't allow deletion of the active config if it's the only one
                if (shippingConfig.IsActive)
                {
                    var otherConfigs = await _context.ShippingConfigs
                        .Where(sc => sc.ShippingConfigID != id)
                        .CountAsync();

                    if (otherConfigs == 0)
                    {
                        TempData["ErrorMessage"] = "Cannot delete the only active shipping configuration. Create a new one first.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Set the most recent other config as active
                    var newActiveConfig = await _context.ShippingConfigs
                        .Where(sc => sc.ShippingConfigID != id)
                        .FirstOrDefaultAsync();

                    if (newActiveConfig != null)
                    {
                        newActiveConfig.IsActive = true;
                    }
                }

                _context.ShippingConfigs.Remove(shippingConfig);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Shipping configuration deleted successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ShippingConfigExists(int id)
        {
            return _context.ShippingConfigs.Any(e => e.ShippingConfigID == id);
        }
    }
} */
