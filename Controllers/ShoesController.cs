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
using Microsoft.AspNetCore.Authorization;

namespace ClothesShopProject.Controllers
{
    public class ShoesController : Controller
    {
        private readonly ClothesShopProjectContext _context;

        public ShoesController(ClothesShopProjectContext context)
        {
            _context = context;
        }

        // GET: Shoes
        public async Task<IActionResult> Index(string Name, string Colour, string Sort)
        {
            IQueryable<Shoe> shoesQuery = _context.Shoe.AsQueryable();

            var colors = shoesQuery.Select(s => s.Colour).Distinct().OrderBy(c => c).ToList();
            var coloursSelectList = new SelectList(colors);

            if (!string.IsNullOrEmpty(Name))
            {
                shoesQuery = shoesQuery.Where(s => s.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Colour) && Colour != "All")
            {
                shoesQuery = shoesQuery.Where(s => s.Colour == Colour);
            }

            if (Sort == "SizeAsc")
            {
                shoesQuery = shoesQuery.OrderBy(s => s.Size);
            }
            else if (Sort == "SizeDesc")
            {
                shoesQuery = shoesQuery.OrderByDescending(s => s.Size);
            }

            var viewModel = new ShoeFilter
            {
                Shoes = await shoesQuery.ToListAsync(),
                Name = Name,
                Colour = Colour,
                Sort = Sort,
                Colours = coloursSelectList
            };

            return View(viewModel);
        }

        // GET: Shoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shoe == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoe
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoe == null)
            {
                return NotFound();
            }

            return View(shoe);
        }


        // GET: Shoes/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var viewModel = new AddOrEditShoe
            {
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Shoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddOrEditShoe viewmodel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (viewmodel.PictureFile != null)
                {
                    uniqueFileName = UploadedFile(viewmodel);
                }

                Shoe shoe = new Shoe
                {
                    Name = viewmodel.Shoe.Name,
                    Colour = viewmodel.Shoe.Colour,
                    Size = viewmodel.Shoe.Size,
                    Picture = uniqueFileName
                };
                _context.Add(shoe);
                await _context.SaveChangesAsync();

                if (viewmodel.ShopId != null && _context.Shop.Any(s => s.Id == viewmodel.ShopId))
                {
                    var shopApparelShoe = new ShopApparelShoe
                    {
                        ApparelId = null,
                        Color = shoe.Colour,
                        ShopId = viewmodel.ShopId.Value,
                        ShoeId = shoe.Id,
                        Price = viewmodel.Price
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

        // GET: Shoes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shoe == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoe
                .Include(s => s.ShopApparelShoes) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (shoe == null)
            {
                return NotFound();
            }

            var viewmodel = new AddOrEditShoe
            {
                Shoe = shoe,
                PictureName = shoe.Picture,
                ShopId = shoe.ShopApparelShoes?.FirstOrDefault()?.ShopId, 
                AvailableShops = _context.Shop.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList()
            };

            return View(viewmodel);
        }

        // POST: Shoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, AddOrEditShoe viewmodel)
        {
            if (id != viewmodel.Shoe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingShoe = await _context.Shoe
                        .Include(s => s.ShopApparelShoes)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingShoe == null)
                    {
                        return NotFound();
                    }

                    string uniqueFileName = existingShoe.Picture;

                    if (viewmodel.PictureFile != null)
                    {
                        if (!string.IsNullOrEmpty(existingShoe.Picture))
                        {
                            string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", existingShoe.Picture);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        uniqueFileName = UploadedFile(viewmodel);
                    }

                    existingShoe.Name = viewmodel.Shoe.Name;
                    existingShoe.Colour = viewmodel.Shoe.Colour;
                    existingShoe.Size = viewmodel.Shoe.Size;
                    existingShoe.Picture = uniqueFileName;

                    if (viewmodel.ShopId.HasValue)
                    {
                        var shopApparelShoes = _context.ShopApparelShoe
                            .Where(sas => sas.ApparelId == existingShoe.Id)
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
                                ShoeId = existingShoe.Id,
                                Color = existingShoe.Colour,
                                Price = 100.99m
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Shoe.Any(e => e.Id == id))
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


        // GET: Shoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shoe == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoe.FirstOrDefaultAsync(m => m.Id == id);
            if (shoe == null)
            {
                return NotFound();
            }

            return View(shoe);
        }

        // POST: Shoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoe = await _context.Shoe
                .Include(s => s.ShopApparelShoes)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shoe == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(shoe.Picture))
            {
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", shoe.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _context.Shoe.Remove(shoe);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private string UploadedFile(AddOrEditShoe viewmodel)
        {
            string uniqueFileName = null;

            if (viewmodel.PictureFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileNameWithoutExt = viewmodel.Shoe.Name;
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
    }
}
