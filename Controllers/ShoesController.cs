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
using System.Data;

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

            // Populate the list of colors
            var colors = shoesQuery.Select(s => s.Colour).Distinct().OrderBy(c => c).ToList();
            var coloursSelectList = new SelectList(colors);

            // Apply the name filter if a name is provided
            if (!string.IsNullOrEmpty(Name))
            {
                shoesQuery = shoesQuery.Where(s => s.Name.Contains(Name));
            }

            // Apply the color filter if a color is selected
            if (!string.IsNullOrEmpty(Colour) && Colour != "All")
            {
                shoesQuery = shoesQuery.Where(s => s.Colour == Colour);
            }

            // Sorting by size
            if (!string.IsNullOrEmpty(Sort) && Sort == "SizeAsc")
            {
                shoesQuery = shoesQuery.OrderBy(s => s.Size);
            }
            else if (!string.IsNullOrEmpty(Sort) && Sort == "SizeDesc")
            {
                shoesQuery = shoesQuery.OrderByDescending(s => s.Size);
            }

            var viewModel = new ShoeFilter
            {
                Shoes = await shoesQuery.ToListAsync(),
                Name = Name,         // Preserve the name filter
                Colour = Colour,     // Preserve the selected color
                Sort = Sort,         // Preserve the selected sorting option
                Colours = coloursSelectList // Set the color dropdown list
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
            return View();
        }

        // POST: Shoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    Picture = uniqueFileName // Assign the unique file name to the Logo property
                };
                _context.Add(shoe);
                await _context.SaveChangesAsync();

                var shopApparelShoe = new ShopApparelShoe
                {
                    ShopId = 140,
                    ApparelId = null,
                    Color = shoe.Colour,
                    ShoeId = shoe.Id,
                    Price = 100.99m,
                };

                _context.ShopApparelShoe.Add(shopApparelShoe);
                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));
            }
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

            var shoe = await _context.Shoe.FindAsync(id);
            if (shoe == null)
            {
                return NotFound();
            }
            AddOrEditShoe viewmodel = new AddOrEditShoe
            {
                Shoe = shoe,
                PictureName = shoe.Picture
            };

            return View(viewmodel);
        }
        // POST: Shoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    if (viewmodel.PictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Shoe.Picture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Shoe.Picture = viewmodel.PictureName;
                    }
                    _context.Update(viewmodel.Shoe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoeExists(viewmodel.Shoe.Id))
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
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAll()
        {
            try
            {
                // Delete all records from the table
                _context.Shoe.RemoveRange(_context.Shoe);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect back to the list view
        }

        // GET: Shoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Shoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shoe == null)
            {
                return Problem("Entity set 'ClothesShopProjectContext.Shoe'  is null.");
            }
            var shoe = await _context.Shoe.FindAsync(id);
            if (shoe != null)
            {
                _context.Shoe.Remove(shoe);
            }
            
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
                // Ensure the file name includes the original file extension
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewmodel.PictureFile.FileName);
                string fileNameWithPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    viewmodel.PictureFile.CopyTo(stream);
                }
            }
            return uniqueFileName;
        }
        private bool ShoeExists(int id)
        {
          return (_context.Shoe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
