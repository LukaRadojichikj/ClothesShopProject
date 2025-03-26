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
                .Include(sas => sas.Shop);

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
                shopApparelShoes = shopApparelShoes.Where(sas => sas.Shop.Name.Contains(filter.Shop));
            }

            var apparelColors = _context.Apparel.Select(a => a.Colour).Distinct().ToList();
            var shoeColors = _context.Shoe.Select(s => s.Colour).Distinct().ToList();
            var allColors = apparelColors.Concat(shoeColors).Distinct().OrderBy(c => c).ToList();

            var colorOptions = allColors.Select(c => new SelectListItem
            {
                Text = c,
                Value = c
            }).ToList();

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
            var viewModel = new ShopApparelShoeFilter
            {
                ShopApparelShoes = shopApparelShoes.ToList(),
                Name = filter.Name,
                Type = filter.Type,
                Shop = filter.Shop,
                Color = filter.Color, 
                ColorOptions = new SelectList(colorOptions, "Value", "Text"),
                SortOptions = filter.SortOptions,
                TypeOptions = filter.TypeOptions
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
                    var existingEntity = await _context.ShopApparelShoe.FindAsync(id);

                    if (existingEntity != null)
                    {
                        existingEntity.Price = model.Price;

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


        public async Task<IActionResult> ApparelShoesByShop(int? Id, string nameSearch, string typeSearch, string sort)
        {
            IQueryable<ShopApparelShoe> shopApparelShoeQuery = _context.ShopApparelShoe.Where(x => x.ShopId == Id)
                .Include(x => x.Apparel)
                .Include(x => x.Shoe)
                .Include(x => x.Shop);



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
                TypeOptions = new SelectList(typeOptions, "Value", "Text"),
                SortOptions = new SelectList(sortOptions, "Value", "Text") 
            };

            return View(ShopApparelShoeFilterVM);
        }

        public IActionResult ApparelShoesBuy(ShopApparelShoeFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Name))
            {
                return NotFound();
            }

            var shopApparelShoes = _context.ShopApparelShoe
                .Include(sas => sas.Apparel)
                .Include(sas => sas.Shoe)
                .Include(sas => sas.Shop)
                .Where(sas => sas.Apparel.Name == filter.Name || sas.Shoe.Name == filter.Name)
                .ToList();

            if (shopApparelShoes == null || shopApparelShoes.Count == 0)
            {
                return NotFound();
            }

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
                .Include(c => c.Shop)
                .Include(c => c.Apparel)
                .Include(c => c.Shoe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopApparelShoe == null)
            {
                return NotFound();
            }

            return View(shopApparelShoe);
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyItemCompleted(long? id, int quantity)
        {
            if (id == null || _context.ShopApparelShoe == null)
            {
                return NotFound();
            }

            var shopApparelShoe = await _context.ShopApparelShoe
                .Include(c => c.Shop)
                .Include(c => c.Apparel)
                .Include(c => c.Shoe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopApparelShoe == null)
            {
                return NotFound();
            }
            var userLoggedInId = HttpContext.Session.GetString("UserLoggedIn");
            var cart = await _context.Cart.FirstOrDefaultAsync(x => x.ClothesShopProjectId.Equals(userLoggedInId));
            if (cart == null)
            {
                cart = new Cart
                {
                    ClothesShopProjectId = userLoggedInId
                };
                _context.Cart.Add(cart);
                await _context.SaveChangesAsync();
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
