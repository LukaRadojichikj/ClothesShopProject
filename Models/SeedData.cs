using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClothesShopProject.Data;
using System;
using System.Linq;
using System.Drawing.Drawing2D;
using Microsoft.AspNetCore.Identity;
using ClothesShopProject.Areas.Identity.Data;

namespace ClothesShopProject.Models
{
    public static class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ClothesShopProjectUser>>();
            IdentityResult roleResult;
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            ClothesShopProjectUser user = await UserManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                var User = new ClothesShopProjectUser();
                User.Email = "admin@admin.com";
                User.UserName = "admin@admin.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
            roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }
            user = await UserManager.FindByEmailAsync("user@user.com");
            if (user == null)
            {
                var User = new ClothesShopProjectUser();
                User.Email = "user@user.com";
                User.UserName = "user@user.com";
                string userPWD = "User1234";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "User"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ClothesShopProjectContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ClothesShopProjectContext>>()))
            {

                CreateUserRoles(serviceProvider).Wait();
                if (context.Shop.Any() || context.Brand.Any() || context.Apparel.Any() || context.ShopApparelShoe.Any())
                {
                    return;
                }

                ClothesShopProjectUser user = context.Users.FirstOrDefault(x => x.UserName.Equals("user@user.com"));
                Cart cart = context.Cart.FirstOrDefault(x => x.ClothesShopProjectId == user.Id);
                
                if (cart == null)
                {
                    Cart newCart = new Cart
                    {
                        ClothesShopProjectId = user.Id
                    };
                    
                    context.Add(newCart);
                    context.SaveChanges();
                }



                context.Shop.AddRange(
               new Shop
               {
                   Name = "Asphalt Gold",
                   Country = "Germany",
                   NumOfShops = 3,
                   Logo = "aglogo.jpg"
               },
               new Shop
               {
                   Name = "Lobby",
                   Country = "Germany",
                   NumOfShops = 18,
                   Logo = "lobbylogo.jpg"
               },
               new Shop
               {
                   Name = "BSTN",
                   Country = "Netherlands",
                   NumOfShops = 8,
                   Logo = "bstnlogo.jpg"
               }
           );
                context.SaveChanges();
                

                context.Brand.AddRange(
                    new Brand
                    {
                        Name = "Nike",
                        Year = "1964",
                        CountryOfOrigin = "USA",
                        Description = "American athletic footwear and apparel corporation headquartered near Beaverton, Oregon",
                        ProfilePicture = "nikelogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id
                    },
                    new Brand
                    {
                        Name = "Adidas",
                        Year = "1949",
                        CountryOfOrigin = "Germany",
                        Description = "German athletic apparel and footwear corporation headquartered in Herzogenaurach, Bavaria, Germany.",
                        ProfilePicture = "adidaslogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id
                    },
                    new Brand
                    {
                        Name = "Carhartt",
                        Year = "1889",
                        CountryOfOrigin = "USA",
                        Description = "American apparel company known for heavy-duty working clothes",
                        ProfilePicture = "carharttlogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id
                    },
                    new Brand
                    {
                        Name = "New Balance",
                        Year = "1906",
                        CountryOfOrigin = "USA",
                        Description = "American apparel company known for heavy-duty working clothes",
                        ProfilePicture = "newbalancelogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id
                    },
                    new Brand
                    {
                        Name = "The North Face",
                        Year = "1966",
                        CountryOfOrigin = "USA",
                        Description = "American outdoor recreation products company.",
                        ProfilePicture = "northfacelogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id
                    },
                    new Brand
                    {
                        Name = "Puma",
                        Year = "1948",
                        CountryOfOrigin = "Germany",
                        Description = "German multinational corporation that designs and manufactures athletic and casual footwear, apparel and accessories",
                        ProfilePicture = "pumalogo.jpg",
                        ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id
                    }
                );

                context.SaveChanges();

                context.Apparel.AddRange(
                       new Apparel
                       {
                           Name = "Carhartt Utah Waistcoat",
                           Colour = "Brown",
                           Picture = "uw.jpg"
                       },
                       new Apparel
                       {
                           Name = "Nike Life Heavy Waffle Longsleeve",
                           Colour = "White",
                           Picture = "nikelhw.jpg"
                       },
                       new Apparel
                       {
                           Name = "New Balance Hoops Fleece Hoodie",
                           Colour = "Green",
                           Picture = "nbhfh.jpg"
                       },
                       new Apparel
                       {
                           Name = "New Balance Hoops Essentials Shorts",
                           Colour = "Blue",
                           Picture = "nbhem.jpg"
                       },
                       new Apparel
                       {
                           Name = "Puma x Rhuigi Double Knee Pants",
                           Colour = "Brown",
                           Picture = "pumadkp.jpg"
                       },
                       new Apparel
                       {
                           Name = "Puma x BMW Longsleeve",
                           Colour = "White",
                           Picture = "pumal.jpg"
                       },
                       new Apparel
                       {
                           Name = "Adidas 3 Stripes Tee",
                           Colour = "Beige",
                           Picture = "adidas3stripes.jpg"
                       },
                       new Apparel
                       {
                           Name = "Adidas x Wales Bonner Tee",
                           Colour = "Yellow",
                           Picture = "adidasbonner.jpg"
                       },
                       new Apparel
                       {
                           Name = "Nike Jordan Essentials Renegade Jacket",
                           Colour = "Brown",
                           Picture = "nikejordanjacket.jpg"
                       },
                       new Apparel
                       {
                           Name = "Nike Authentics Lined Coaches Jacket",
                           Colour = "Blue",
                           Picture = "nikealc.jpg"
                       },
                       new Apparel
                       {
                           Name = "The North Face Gore-Tex Mountain Pant",
                           Colour = "Black",
                           Picture = "northfacepant.jpg"
                       },
                       new Apparel
                       {
                           Name = "The North Face Gore-Tex Mountain Guide Insulated",
                           Colour = "Yellow",
                           Picture = "northfacejacket.jpg"
                       }
                   );
                context.SaveChanges();

                context.Shoe.AddRange(
                       new Shoe
                       {
                           Name = "Puma Clyde Vintage",
                           Colour = "White",
                           Size = 41,
                           Picture = "pumaclyde.jpg"
                       },
                       new Shoe
                       {
                           Name = "Puma Velophasis Technish",
                           Colour = "Black",
                           Size = 41,
                           Picture = "pumavelo.jpg"
                       },
                       new Shoe
                       {
                           Name = "Puma Palermo OG",
                           Colour = "Blue",
                           Size = 43,
                           Picture = "pumapalermo.jpg"
                       },
                       new Shoe
                       {
                           Name = "New Balance ML610TBI",
                           Colour = "Brown",
                           Size = 43,
                           Picture = "nbml.jpg"
                       },
                       new Shoe
                       {
                           Name = "New Balance BB480LWA",
                           Colour = "Yellow",
                           Size = 45,
                           Picture = "nbbb.jpg"
                       },
                       new Shoe
                       {
                           Name = "The North Face NSE Low",
                           Colour = "Green",
                           Size = 40,
                           Picture = "northfaceface.jpg"
                       },
                       new Shoe
                       {
                           Name = "The North Face Nuptse Mule",
                           Colour = "Green",
                           Size = 40,
                           Picture = "northfacemule.jpg"
                       },
                       new Shoe
                       {
                           Name = "Adidas Samba OG",
                           Colour = "Black",
                           Size = 45,
                           Picture = "adidassamba.jpg"
                       },
                       new Shoe
                       {
                           Name = "Adidas Gazelle",
                           Colour = "Brown",
                           Size = 42,
                           Picture = "adidasgazelle.jpg"
                       },
                       new Shoe
                       {
                           Name = "Nike Terminator Low",
                           Colour = "Blue",
                           Size = 41,
                           Picture = "niketerminator.jpg"
                       }, new Shoe
                       {
                           Name = "Nike Dunk High Retro",
                           Colour = "Green",
                           Size = 43,
                           Picture = "nikedunkhigh.jpg"
                       }
                   );
                context.SaveChanges();

                context.ShopApparelShoe.AddRange(
                        new ShopApparelShoe
                        {
                            ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id,
                            ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Carhartt Utah Waistcoat").Id,
                            ShoeId = null,
                            Price = 279.99m
                        },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Puma Clyde Vintage").Id,
                           Price = 109.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Nike Life Heavy Waffle Longsleeve").Id,
                           ShoeId = null,
                           Price = 69.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "New Balance Hoops Fleece Hoodie").Id,
                           ShoeId = null,
                           Price = 79.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "New Balance Hoops Essentials Shorts").Id,
                           ShoeId = null,
                           Price = 49.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Puma x Rhuigi Double Knee Pants").Id,
                           ShoeId = null,
                           Price = 69.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Puma x BMW Longsleeve").Id,
                           ShoeId = null,
                           Price = 59.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Adidas 3 Stripes Tee").Id,
                           ShoeId = null,
                           Price = 35.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Adidas x Wales Bonner Tee").Id,
                           ShoeId = null,
                           Price = 59.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Nike Jordan Essentials Renegade Jacket").Id,
                           ShoeId = null,
                           Price = 249.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "Nike Authentics Lined Coaches Jacket").Id,
                           ShoeId = null,
                           Price = 109.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "The North Face Gore-Tex Mountain Pant").Id,
                           ShoeId = null,
                           Price = 249.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id,
                           ApparelId = context.Apparel.FirstOrDefault(x => x.Name == "The North Face Gore-Tex Mountain Guide Insulated").Id,
                           ShoeId = null,
                           Price = 699.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Puma Velophasis Technish").Id,
                           Price = 79.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Puma Palermo OG").Id,
                           Price = 99.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "New Balance ML610TBI").Id,
                           Price = 129.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "BSTN").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "New Balance BB480LWA").Id,
                           Price = 99.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "The North Face NSE Low").Id,
                           Price = 59.99m    
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Lobby").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "The North Face Nuptse Mule").Id,
                           Price = 79.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Adidas Samba OG").Id,
                           Price = 149.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Adidas Gazelle").Id,
                           Price = 109.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Nike Terminator Low").Id,
                           Price = 129.99m
                       },
                       new ShopApparelShoe
                       {
                           ShopId = context.Shop.FirstOrDefault(x => x.Name == "Asphalt Gold").Id,
                           ApparelId = null,
                           ShoeId = context.Shoe.FirstOrDefault(a => a.Name == "Nike Dunk High Retro").Id,
                           Price = 129.99m
                       }

                   );
              context.SaveChanges();

            }
        }
    }
}