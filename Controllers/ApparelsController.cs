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
        public async Task<IActionResult> Index(string Name, string Colour)
        {
            IQueryable<Apparel> apparelsQuery = _context.Apparel.AsQueryable();

            // Populate the list of colors
            var colors = apparelsQuery.Select(a => a.Colour).Distinct().OrderBy(c => c).ToList();
            var coloursSelectList = new SelectList(colors);

            // Apply the name filter if a name is provided
            if (!string.IsNullOrEmpty(Name))
            {
                apparelsQuery = apparelsQuery.Where(a => a.Name.Contains(Name));
            }

            // Apply the color filter if a color is selected
            if (!string.IsNullOrEmpty(Colour))
            {
                apparelsQuery = apparelsQuery.Where(a => a.Colour == Colour);
            }

            var viewModel = new ApparelFilter
            {
                Apparels = await apparelsQuery.ToListAsync(),
                Name = Name, // Preserve the name filter
                Colour = Colour, // Preserve the selected color
                Colours = coloursSelectList // Set the color dropdown list
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

        // GET: Apparels/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Apparels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    Picture = uniqueFileName // Assign the unique file name to the Logo property
                };
                _context.Add(apparel);
                await _context.SaveChangesAsync();

                var shopApparelShoe = new ShopApparelShoe
                {
                    ShopId = 140,
                    ApparelId = apparel.Id,
                    Color = apparel.Colour,
                    ShoeId = null,
                    Price = 100.99m,
                };

                _context.ShopApparelShoe.Add(shopApparelShoe);
                await _context.SaveChangesAsync();



                return RedirectToAction(nameof(Index));
            }
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

            var apparel = await _context.Apparel.FindAsync(id);
            if (apparel == null)
            {
                return NotFound();
            }
            AddOrEditApparel viewmodel = new AddOrEditApparel
            {
                Apparel = apparel,
                PictureName = apparel.Picture
            };

            return View(viewmodel);
        }

        // POST: Apparels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    if (viewmodel.PictureFile != null)
                    {
                        string uniqueFileName = UploadedFile(viewmodel);
                        viewmodel.Apparel.Picture = uniqueFileName;
                    }
                    else
                    {
                        viewmodel.Apparel.Picture = viewmodel.PictureName;
                    }
                    _context.Update(viewmodel.Apparel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApparelExists(viewmodel.Apparel.Id))
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
                _context.Apparel.RemoveRange(_context.Apparel);
                _context.SaveChanges();
                TempData["Message"] = "All records deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting records: {ex.Message}";
            }

            return RedirectToAction("Index"); // Redirect back to the list view
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
            if (_context.Apparel == null)
            {
                return Problem("Entity set 'ClothesShopProjectContext.Apparel'  is null.");
            }
            var apparel = await _context.Apparel.FindAsync(id);
            if (apparel != null)
            {
                _context.Apparel.Remove(apparel);
            }
            
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
        private bool ApparelExists(int id)
        {
          return (_context.Apparel?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
