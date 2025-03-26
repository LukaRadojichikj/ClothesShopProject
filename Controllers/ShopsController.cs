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
                    Logo = uniqueFileName 
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
                    var existingShop = await _context.Shop.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

                    if (existingShop == null)
                    {
                        return NotFound();
                    }

                    string uniqueFileName = existingShop.Logo;

                    if (viewmodel.LogoFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingShop.Logo))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", existingShop.Logo);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        uniqueFileName = UploadedFile(viewmodel);
                    }

                    viewmodel.Shop.Logo = uniqueFileName; 

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

            var shop = await _context.Shop.FirstOrDefaultAsync(m => m.Id == id);
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
                _context.Shop.RemoveRange(_context.Shop);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shop = await _context.Shop
                .Include(s => s.Brands) 
                .Include(s => s.ShopApparelShoes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(shop.Logo)) 
            {
                string shopImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", shop.Logo);
                if (System.IO.File.Exists(shopImagePath))
                {
                    System.IO.File.Delete(shopImagePath);
                }
            }

            foreach (var brand in shop.Brands)
            {
                if (!string.IsNullOrEmpty(brand.ProfilePicture))
                {
                    string brandImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", brand.ProfilePicture);
                    if (System.IO.File.Exists(brandImagePath))
                    {
                        System.IO.File.Delete(brandImagePath);
                    }
                }
            }

            var apparelIds = shop.ShopApparelShoes.Select(sas => sas.ApparelId).ToList();
            var apparels = await _context.Apparel
                .Where(a => apparelIds.Contains(a.Id))
                .ToListAsync();

            foreach (var apparel in apparels)
            {
                if (!string.IsNullOrEmpty(apparel.Picture))
                {
                    string apparelImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", apparel.Picture);
                    if (System.IO.File.Exists(apparelImagePath))
                    {
                        System.IO.File.Delete(apparelImagePath);
                    }
                }
            }

            var shoeIds = shop.ShopApparelShoes.Select(sas => sas.ShoeId).ToList();
            var shoes = await _context.Shoe
                .Where(s => shoeIds.Contains(s.Id))
                .ToListAsync();

            foreach (var shoe in shoes)
            {
                if (!string.IsNullOrEmpty(shoe.Picture))
                {
                    string shoeImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", shoe.Picture);
                    if (System.IO.File.Exists(shoeImagePath))
                    {
                        System.IO.File.Delete(shoeImagePath);
                    }
                }
            }

            _context.Brand.RemoveRange(shop.Brands);

            _context.Apparel.RemoveRange(apparels);
            _context.Shoe.RemoveRange(shoes);

            _context.Shop.Remove(shop);
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

                string fileNameWithoutExt = viewmodel.Shop.Name;
                fileNameWithoutExt = string.Concat(fileNameWithoutExt.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");

                string fileExtension = Path.GetExtension(viewmodel.LogoFile.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

                uniqueFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
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
