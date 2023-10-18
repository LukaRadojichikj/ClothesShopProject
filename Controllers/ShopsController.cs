using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClothesShopProject.Data;
using ClothesShopProject.Models;
using ClothesShopProject.ViewModels;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ClothesShopProject.Controllers
{
    public class ShopsController : Controller
    {
        private readonly ClothesShopProjectContext _context;

        public ShopsController(ClothesShopProjectContext context)
        {
            _context = context;
        }

        // GET: Shops
        public async Task<IActionResult> Index(string Name)
        {
            var shops = _context.Shop.AsQueryable();

            // Apply the search filter if the NameSearch property is not null or empty
            if (!string.IsNullOrEmpty(Name))
            {
                shops = shops.Where(s => s.Name.Contains(Name));
            }

            shops = shops.OrderBy(s => s.Name);

            var ShopFilterVM = new ShopFilter
            {
                Shops = shops.ToList(),
                Name = Name
            };

            return View(ShopFilterVM);
        }

        // GET: Shops/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shop == null)
            {
                return NotFound();
            }

            var shop = await _context.Shop
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shop == null)
            {
                return NotFound();
            }

            return View(shop);
        }

        // GET: Shops/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditShop viewmodel)
        {
            if (ModelState.IsValid)
            {
                
                string uniqueFileName = null;
                if (viewmodel.LogoFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }

                Shop shop = new Shop
                {
                    Name = viewmodel.Shop.Name,
                    Country = viewmodel.Shop.Country,
                    NumOfShops = viewmodel.Shop.NumOfShops,
                    Logo = uniqueFileName // Assign the unique file name to the Logo property
                };

                _context.Add(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewmodel);
        }


        // GET: Shops/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shop == null)
            {
                return NotFound();
            }

            var shop = await _context.Shop.FindAsync(id);
            if (shop == null)
            {
                return NotFound();
            }
            AddOrEditShop viewmodel = new AddOrEditShop
            {
                Shop = shop,
                LogoName = shop.Logo
            };

            return View(viewmodel);
        }

        // POST: Shops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditShop viewmodel)
        {
            if (id != viewmodel.Shop.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.LogoFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Shop.Logo = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Shop.Logo = viewmodel.LogoName;
                    }
                    _context.Update(viewmodel.Shop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopExists(viewmodel.Shop.Id))
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
            return View(viewmodel);
        }

        // GET: Shops/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shop == null)
            {
                return NotFound();
            }

            var shop = await _context.Shop
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shop == null)
            {
                return NotFound();
            }

            return View(shop);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAll()
        {
            try
            {
                // Delete all records from the table
                _context.Shop.RemoveRange(_context.Shop);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect back to the list view
        }

        // POST: Shops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shop == null)
            {
                return Problem("Entity set 'ClothesShopProjectContext.Shop'  is null.");
            }
            var shop = await _context.Shop.FindAsync(id);
            if (shop != null)
            {
                _context.Shop.Remove(shop);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string UploadedFile(AddOrEditShop viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.LogoFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                // Ensure the file name includes the original file extension
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.LogoFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.LogoFile.CopyTo(stream);  
                }
            }
            return uniqueFileName;
        }


        private bool ShopExists(int id)
        {
          return (_context.Shop?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
