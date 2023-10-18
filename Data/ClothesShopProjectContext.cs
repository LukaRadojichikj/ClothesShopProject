using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ClothesShopProject.Models;
using ClothesShopProject.Areas.Identity.Data;

namespace ClothesShopProject.Data
{
    public class ClothesShopProjectContext : IdentityDbContext<ClothesShopProjectUser>
    {
        public ClothesShopProjectContext (DbContextOptions<ClothesShopProjectContext> options)
            : base(options)
        {
        }
       
        public DbSet<ClothesShopProject.Models.Brand> Brand { get; set; }

        public DbSet<ClothesShopProject.Models.Shop>? Shop { get; set; }

        public DbSet<ClothesShopProject.Models.Apparel>? Apparel { get; set; }

        public DbSet<ClothesShopProject.Models.Shoe>? Shoe { get; set; }

        public DbSet<ClothesShopProject.Models.ShopApparelShoe>? ShopApparelShoe { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<ClothesShopProject.Models.Cart>? Cart { get; set; }

        public DbSet<ClothesShopProject.Models.CartItem>? CartItem { get; set; }


    }

}
