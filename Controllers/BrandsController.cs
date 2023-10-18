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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ClothesShopProject.Controllers
{
    public class BrandsController : Controller
    {
        private readonly ClothesShopProjectContext _context;

        public BrandsController(ClothesShopProjectContext context)
        {
            _context = context;
        }

        // GET: Brands
        public async Task<IActionResult> Index(string Name, string Shop)
        {
            IQueryable<Brand> brandsQuery = _context.Brand.AsQueryable().Include(x => x.Shop);
            IQueryable<string> shopQuery = _context.Brand.OrderBy(x => x.Shop.Name).Select(x => x.Shop.Name).Distinct();

            if (!string.IsNullOrEmpty(Name))
            {
                brandsQuery = brandsQuery.Where(x => x.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Shop))
            {
                brandsQuery = brandsQuery.Where(x => x.Shop.Name.Contains(Shop));
            }

            var BrandFilterVM = new BrandFilter
            {
                Brands = await brandsQuery.Include(x => x.Shop).ToListAsync(),
                Shops = new SelectList(await shopQuery.ToListAsync())
            };

            return View(BrandFilterVM);
        }

        // GET: Brands/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .Include(b => b.Shop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // GET: Brands/Create
        // GET: Brands/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Get a list of shops for the dropdown
            var shops = _context.Shop.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            // Set ViewBag.ShopId to the list of shops
            ViewBag.ShopId = new SelectList(shops, "Value", "Text");

            return View();
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditBrand viewmodel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (viewmodel.ProfilePictureFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }

                Brand brand = new Brand
                {
                    Name = viewmodel.Brand.Name,
                    Year = viewmodel.Brand.Year,
                    CountryOfOrigin = viewmodel.Brand.CountryOfOrigin,
                    Description = viewmodel.Brand.Description,
                    ProfilePicture = uniqueFileName,
                    ShopId = viewmodel.Brand.ShopId
                };

                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Get a list of shops for the dropdown again
            var shops = _context.Shop.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            // Set ViewBag.ShopId to the list of shops
            ViewBag.ShopId = new SelectList(shops, "Value", "Text");

            return View(viewmodel);
        }

        // GET: Brands/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            AddOrEditBrand viewmodel = new AddOrEditBrand
            {
                Brand = brand,
                ProfilePictureName = brand.ProfilePicture
            };

            ViewData["ShopId"] = new SelectList(_context.Shop, "Id", "Name", brand.ShopId);
            return View(viewmodel);
        }


        // POST: Brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditBrand viewmodel)
        {
            if (id != viewmodel.Brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (viewmodel.ProfilePictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Brand.ProfilePicture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Brand.ProfilePicture = viewmodel.ProfilePictureName;
                    }
                    _context.Update(viewmodel.Brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(viewmodel.Brand.Id))
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
            ViewData["ShopId"] = new SelectList(_context.Brand, "Id", "Name", viewmodel.Brand.ShopId);
            return View(viewmodel);
        }


        // GET: Brands/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Brand == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .Include(b => b.Shop)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAll()
        {
            try
            {
                // Delete all records from the table
                _context.Brand.RemoveRange(_context.Brand);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect back to the list view
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Brand == null)
            {
                return Problem("Entity set 'ClothesShopProjectContext.Brand'  is null.");
            }
            var brand = await _context.Brand.FindAsync(id);
            if (brand != null)
            {
                _context.Brand.Remove(brand);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrandExists(int id)
        {
            return (_context.Brand?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string UploadedFile(AddOrEditBrand viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.ProfilePictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.ProfilePictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.ProfilePictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }

        public async Task<IActionResult> BrandsByShop(int? Id, string Name, string Shop)
        {
            IQueryable<Brand> brandsQuery = _context.Brand.Where(x => x.ShopId == Id).Include(x => x.Shop);
            IQueryable<string> shopQuery = _context.Brand.OrderBy(x => x.Shop.Name).Select(x => x.Shop.Name).Distinct();

            var shop = await _context.Shop.FirstOrDefaultAsync(m => m.Id == Id);
            ViewBag.Message = shop.Name;

            if (!string.IsNullOrEmpty(Name))
            {
                brandsQuery = brandsQuery.Where(x => x.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Shop))
            {
                brandsQuery = brandsQuery.Where(x => x.Shop.Name.Contains(Shop));
            }

            var BrandFilterVM = new BrandFilter
            {
                Brands = await brandsQuery.Include(x => x.Shop).ToListAsync(),
                Shops = new SelectList(await shopQuery.ToListAsync())
            };

            return View(BrandFilterVM);
        }
    }
}