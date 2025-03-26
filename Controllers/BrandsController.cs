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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new AddOrEditBrand
            {
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Brands/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditBrand viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = null;

                    if (viewModel.ProfilePictureFile != null)
                    {
                        uniqueFileName = UploadedFile(viewModel);
                    }

                    var brand = new Brand
                    {
                        Name = viewModel.Brand.Name,
                        Year = viewModel.Brand.Year,
                        CountryOfOrigin = viewModel.Brand.CountryOfOrigin,
                        Description = viewModel.Brand.Description,
                        ProfilePicture = uniqueFileName,
                        ShopId = viewModel.ShopId 
                    };

                    _context.Add(brand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the brand.");
                }
            }

            viewModel.AvailableShops = _context.Shop.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            return View(viewModel);
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
                ProfilePictureName = brand.ProfilePicture,
                ShopId = brand.ShopId,
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewmodel);
        }


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
                    var existingBrand = await _context.Brand.FindAsync(id);

                    if (existingBrand == null)
                    {
                        return NotFound();
                    }

                    string uniqueFileName = existingBrand.ProfilePicture; 

                    if (viewmodel.ProfilePictureFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingBrand.ProfilePicture))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", existingBrand.ProfilePicture);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        uniqueFileName = UploadedFile(viewmodel);
                    }
                    existingBrand.Name = viewmodel.Brand.Name;
                    existingBrand.Year = viewmodel.Brand.Year;
                    existingBrand.Description = viewmodel.Brand.Description;
                    existingBrand.CountryOfOrigin = viewmodel.Brand.CountryOfOrigin;
                    existingBrand.ShopId = viewmodel.ShopId;
                    existingBrand.ProfilePicture = uniqueFileName;


                    _context.Update(existingBrand);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
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
            }

            viewmodel.AvailableShops = _context.Shop.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
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
                _context.Brand.RemoveRange(_context.Brand);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); 
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brand.FindAsync(id);

            if (brand != null)
            {
                if (!string.IsNullOrEmpty(brand.ProfilePicture))
                {
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", brand.ProfilePicture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Brand.Remove(brand);
                await _context.SaveChangesAsync();
            }

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

                string fileNameWithoutExt = viewmodel.Brand.Name;
                fileNameWithoutExt = string.Concat(fileNameWithoutExt.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");

                string fileExtension = Path.GetExtension(viewmodel.ProfilePictureFile.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

                uniqueFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
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