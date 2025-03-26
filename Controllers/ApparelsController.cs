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
using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ClothesShopProject.Controllers
{
    public class ApparelsController : Controller
    {
        private readonly ClothesShopProjectContext _context;

        public ApparelsController(ClothesShopProjectContext context)
        {
            _context = context;
        }

        // GET: Apparels
        public async Task<IActionResult> Index(string Name, string Colour, int? BrandId)
        {
            IQueryable<Apparel> apparelsQuery = _context.Apparel
                .AsQueryable();

            var colors = apparelsQuery.Select(a => a.Colour).Distinct().OrderBy(c => c).ToList();
            var coloursSelectList = new SelectList(colors);

            var brands = _context.Brand.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            if (!string.IsNullOrEmpty(Name))
            {
                apparelsQuery = apparelsQuery.Where(a => a.Name.Contains(Name));
            }

            if (!string.IsNullOrEmpty(Colour))
            {
                apparelsQuery = apparelsQuery.Where(a => a.Colour == Colour);
            }


            var viewModel = new ApparelFilter
            {
                Apparels = await apparelsQuery.ToListAsync(),
                Name = Name,
                Colour = Colour,
                Colours = coloursSelectList,
                BrandId = BrandId,
                Brands = new SelectList(brands, "Value", "Text")
            };

            return View(viewModel);
        }


        // GET: Apparels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Apparel == null)
            {
                return NotFound();
            }

            var apparel = await _context.Apparel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apparel == null)
            {
                return NotFound();
            }

            return View(apparel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new AddOrEditApparel
            {
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditApparel viewmodel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (viewmodel.PictureFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }

                Apparel apparel = new Apparel
                {
                    Name = viewmodel.Apparel.Name,
                    Colour = viewmodel.Apparel.Colour,
                    Picture = uniqueFileName
                };
                _context.Add(apparel);
                await _context.SaveChangesAsync();

                if (viewmodel.ShopId != null && _context.Shop.Any(b => b.Id == viewmodel.ShopId))
                {
                    var shopApparelShoe = new ShopApparelShoe
                    {
                        ApparelId = apparel.Id, 
                        ShopId = viewmodel.ShopId.Value, 
                        Price = viewmodel.Price, 
                        Color = apparel.Colour
                    };

                    _context.ShopApparelShoe.Add(shopApparelShoe);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("ShopId", "Invalid Shop selection.");
                }

                return RedirectToAction(nameof(Index));
            }

            viewmodel.AvailableShops = _context.Shop.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            return View(viewmodel);
        }


        // GET: Apparels/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Apparel == null)
            {
                return NotFound();
            }

            var apparel = await _context.Apparel
                .Include(a => a.ShopApparelShoes) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (apparel == null)
            {
                return NotFound();
            }

            var viewModel = new AddOrEditApparel
            {
                Apparel = apparel,
                PictureName = apparel.Picture,
                ShopId = apparel.ShopApparelShoes?.FirstOrDefault()?.ShopId,
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Apparels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditApparel viewmodel)
        {
            if (id != viewmodel.Apparel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingApparel = await _context.Apparel
                        .Include(a => a.ShopApparelShoes)
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (existingApparel == null)
                    {
                        return NotFound();
                    }

                    string uniqueFileName = existingApparel.Picture;

                    if (viewmodel.PictureFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingApparel.Picture))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", existingApparel.Picture);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        uniqueFileName = UploadedFile(viewmodel);
                    }

                    existingApparel.Name = viewmodel.Apparel.Name;
                    existingApparel.Colour = viewmodel.Apparel.Colour;
                    existingApparel.Picture = uniqueFileName;

                    if (viewmodel.ShopId.HasValue)
                    {
                        var shopApparelShoes = _context.ShopApparelShoe
                            .Where(sas => sas.ApparelId == existingApparel.Id)
                            .ToList();

                        if (shopApparelShoes.Any())
                        {
                            foreach (var shopApparelShoe in shopApparelShoes)
                            {
                                shopApparelShoe.ShopId = viewmodel.ShopId.Value;
                            }
                        }
                        else
                        {
                            _context.ShopApparelShoe.Add(new ShopApparelShoe
                            {
                                ShopId = viewmodel.ShopId.Value,
                                ApparelId = existingApparel.Id,
                                Color = existingApparel.Colour,
                                Price = 100.99m
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Apparel.Any(e => e.Id == id))
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

            viewmodel.AvailableShops = _context.Shop.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();

            return View(viewmodel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAll()
        {
            try
            {
                _context.Apparel.RemoveRange(_context.Apparel);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); 
        }

        // GET: Apparels/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Apparel == null)
            {
                return NotFound();
            }

            var apparel = await _context.Apparel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (apparel == null)
            {
                return NotFound();
            }

            return View(apparel);
        }

        // POST: Apparels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apparel = await _context.Apparel.FindAsync(id);

            if (apparel == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(apparel.Picture))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", apparel.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Apparel.Remove(apparel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private string UploadedFile(AddOrEditApparel viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.PictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileNameWithoutExt = viewmodel.Apparel.Name;
                fileNameWithoutExt = string.Concat(fileNameWithoutExt.Split(Path.GetInvalidFileNameChars()))
                                           .Replace(" ", "_"); 

                string fileExtension = Path.GetExtension(viewmodel.PictureFile.FileName).ToLower();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
                }

                uniqueFileName = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    viewmodel.PictureFile.CopyTo(stream);
                }
            }

            return uniqueFileName;
        }


        private bool ApparelExists(int id)
        {
          return (_context.Apparel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
