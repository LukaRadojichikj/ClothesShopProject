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
    public class ShopApparelShoesController : Controller
    {
        private readonly ClothesShopProjectContext _context;

        public ShopApparelShoesController(ClothesShopProjectContext context)
        {
            _context = context;
        }


        public IActionResult Index(ShopApparelShoeFilter filter)
        {
            IQueryable<ShopApparelShoe> shopApparelShoes = _context.ShopApparelShoe
                .Include(sas => sas.Apparel)
                .Include(sas => sas.Shoe)
                .Include(sas => sas.Shops);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Name))
            {
                shopApparelShoes = shopApparelShoes.Where(sas => sas.Apparel.Name.Contains(filter.Name) || sas.Shoe.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrEmpty(filter.Type))
            {
                if (filter.Type == "Apparel")
                {
                    shopApparelShoes = shopApparelShoes.Where(sas => sas.ApparelId != null);
                }
                else if (filter.Type == "Shoe")
                {
                    shopApparelShoes = shopApparelShoes.Where(sas => sas.ShoeId != null);
                }
            }

            if (!string.IsNullOrEmpty(filter.Shop))
            {
                shopApparelShoes = shopApparelShoes.Where(sas => sas.Shops.Name.Contains(filter.Shop));
            }

           var colorOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Brown", Value = "Brown" },
                new SelectListItem { Text = "Blue", Value = "Blue" },
                new SelectListItem { Text = "White", Value = "White" },
                new SelectListItem { Text = "Green", Value = "Green" },
                new SelectListItem { Text = "Beige", Value = "Beige" },
                new SelectListItem { Text = "Yellow", Value = "Yellow" },
                new SelectListItem { Text = "Black", Value = "Black" },
                // Add more color options as needed
            };



            // Apply sorting
            if (!string.IsNullOrEmpty(filter.Sort))
            {
                if (filter.Sort == "Ascending")
                {
                    shopApparelShoes = shopApparelShoes.OrderBy(sas => sas.Price);
                }
                else if (filter.Sort == "Descending")
                {
                    shopApparelShoes = shopApparelShoes.OrderByDescending(sas => sas.Price);
                }
            }
            filter.TypeOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Apparel", Value = "Apparel" },
                new SelectListItem { Text = "Shoe", Value = "Shoe" },
            }, "Value", "Text");

            // Define options for the Sort dropdown
            filter.SortOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Ascending", Value = "Ascending" },
                new SelectListItem { Text = "Descending", Value = "Descending" },
            }, "Value", "Text");

            if (!string.IsNullOrEmpty(filter.Color))
            {
                shopApparelShoes = shopApparelShoes.Where(sas =>
                    (sas.Apparel != null && sas.Apparel.Colour == filter.Color) ||
                    (sas.Shoe != null && sas.Shoe.Colour == filter.Color));
            }

            // Create the ViewModel with filtered results
            var viewModel = new ShopApparelShoeFilter
            {
                ShopApparelShoes = shopApparelShoes.ToList(),
                Name = filter.Name,
                Type = filter.Type,
                Shop = filter.Shop,
                Color = filter.Color, // Set the selected color
                ColorOptions = new SelectList(colorOptions, "Value", "Text"),
                SortOptions = filter.SortOptions,
                TypeOptions = filter.TypeOptions
                // Populate other properties as needed
            };

            return View(viewModel);
        }



        // GET: ShopApparelShoes/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.ShopApparelShoe == null)
            {
                return NotFound();
            }

            var shopApparelShoe = await _context.ShopApparelShoe.FindAsync(id);

            if (shopApparelShoe == null)
            {
                return NotFound();
            }


            return View(shopApparelShoe);
        }

        // POST: ShopApparelShoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        // POST: YourController/Edit/5
        public async Task<IActionResult> Edit(long id, [Bind("Id, Price")] ShopApparelShoe model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing entity from the database
                    var existingEntity = await _context.ShopApparelShoe.FindAsync(id);

                    if (existingEntity != null)
                    {
                        // Update only the Price property
                        existingEntity.Price = model.Price;

                        // Update the entity in the context and save changes
                        _context.Update(existingEntity);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopApparelShoeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }




        // GET: ShopApparelShoes/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.ShopApparelShoe == null)
            {
                return NotFound();
            }

            var shopApparelShoe = await _context.ShopApparelShoe
                .Include(s => s.Apparel)
                .Include(s => s.Shoe)
                .Include(s => s.Shops)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopApparelShoe == null)
            {
                return NotFound();
            }

            return View(shopApparelShoe);
        }

        // POST: ShopApparelShoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.ShopApparelShoe == null)
            {
                return Problem("Entity set 'ClothesShopProjectContext.ShopApparelShoe'  is null.");
            }
            var shopApparelShoe = await _context.ShopApparelShoe.FindAsync(id);
            if (shopApparelShoe != null)
            {
                _context.ShopApparelShoe.Remove(shopApparelShoe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ApparelShoesByShop(int? Id, string nameSearch, string typeSearch, string sort)
        {
            IQueryable<ShopApparelShoe> shopApparelShoeQuery = _context.ShopApparelShoe.Where(x => x.ShopId == Id)
                .Include(x => x.Apparel)
                .Include(x => x.Shoe)
                .Include(x => x.Shops);

            

            var typeOptions = new List<SelectListItem>
    {
        new SelectListItem { Text = "All", Value = "" },
        new SelectListItem { Text = "Apparel", Value = "Apparel" },
        new SelectListItem { Text = "Shoe", Value = "Shoe" }
    };

            var sortOptions = new List<SelectListItem>
    {
        new SelectListItem { Text = "All", Value = "" },
        new SelectListItem { Text = "Ascending", Value = "Ascending" },
        new SelectListItem { Text = "Descending", Value = "Descending" }
    };

            var shop = await _context.Shop.FirstOrDefaultAsync(m => m.Id == Id);
            ViewBag.Message = shop.Name;

            if (!string.IsNullOrEmpty(nameSearch))
            {
                shopApparelShoeQuery = shopApparelShoeQuery.Where(x => x.Apparel.Name.Contains(nameSearch) || x.Shoe.Name.Contains(nameSearch));
            }



            if (!string.IsNullOrEmpty(typeSearch))
            {
                if (typeSearch == "Apparel")
                {
                    shopApparelShoeQuery = shopApparelShoeQuery.Where(x => x.ApparelId != null);
                }
                else if (typeSearch == "Shoe")
                {
                    shopApparelShoeQuery = shopApparelShoeQuery.Where(x => x.ShoeId != null);
                }
            }

            if (!string.IsNullOrEmpty(sort))
            {
                if (string.Compare(sort, "Ascending") == 0)
                {
                    shopApparelShoeQuery = shopApparelShoeQuery.OrderBy(x => x.Price);
                }
                else
                {
                    shopApparelShoeQuery = shopApparelShoeQuery.OrderByDescending(x => x.Price);
                }
            }

            var ShopApparelShoeFilterVM = new ShopApparelShoeFilter
            {
                ShopApparelShoes = await shopApparelShoeQuery.ToListAsync(),
                Name = nameSearch,
                Type = typeSearch,
                Sort = sort,
                TypeOptions = new SelectList(typeOptions, "Value", "Text"), // Corrected property name
                SortOptions = new SelectList(sortOptions, "Value", "Text") // Corrected property name
            };

            return View(ShopApparelShoeFilterVM);
        }

        public IActionResult ApparelShoesBuy(ShopApparelShoeFilter filter)
        {
            // Check if the filter parameters are null or empty
            if (string.IsNullOrEmpty(filter.Name))
            {
                return NotFound();
            }

            // Retrieve the specific apparel or shoe items by name
            var shopApparelShoes = _context.ShopApparelShoe
                .Include(sas => sas.Apparel)
                .Include(sas => sas.Shoe)
                .Include(sas => sas.Shops)
                .Where(sas => sas.Apparel.Name == filter.Name || sas.Shoe.Name == filter.Name)
                .ToList();

            if (shopApparelShoes == null || shopApparelShoes.Count == 0)
            {
                return NotFound();
            }

            // Pass the filtered items to a view
            filter.ShopApparelShoes = shopApparelShoes;
            return View(filter);
        }


        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyItem(long? id)
        {
            if (id == null || _context.ShopApparelShoe == null)
            {
                return NotFound();
            }

            var shopApparelShoe = await _context.ShopApparelShoe
                .Include(c => c.Shops)
                .Include(c => c.Apparel)
                .Include(c=> c.Shoe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopApparelShoe == null)
            {
                return NotFound();
            }

            return View(shopApparelShoe);
        }

        // GET: CompanyFlights/BookFlightCompleted/5
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyItemCompleted(long? id, int quantity)
        {
            if (id == null || _context.ShopApparelShoe == null)
            {
                return NotFound();
            }

            var shopApparelShoe = await _context.ShopApparelShoe
                .Include(c => c.Shops)
                .Include(c => c.Apparel)
                .Include(c=> c.Shoe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopApparelShoe == null)
            {
                return NotFound();
            }
            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.ClothesShopProjectId.Equals(userLoggedInId));
            if (cart == null)
            {
                return NotFound();
            }
            CartItem cartItem = new CartItem
            {
                CartId = cart.Id,
                ShopApparelShoeId = id,
                Quantity = quantity
            };

            _context.Add(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ShopApparelShoeExists(long id)
        {
          return (_context.ShopApparelShoe?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
