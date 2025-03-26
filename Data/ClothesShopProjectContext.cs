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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Apparel → ShopApparelShoe (CASCADE DELETE)
            modelBuilder.Entity<ShopApparelShoe>()
                .HasOne(sas => sas.Apparel)
                .WithMany(a => a.ShopApparelShoes)
                .HasForeignKey(sas => sas.ApparelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Shoe → ShopApparelShoe (CASCADE DELETE)
            modelBuilder.Entity<ShopApparelShoe>()
                .HasOne(sas => sas.Shoe)
                .WithMany(s => s.ShopApparelShoes)
                .HasForeignKey(sas => sas.ShoeId)
                .OnDelete(DeleteBehavior.Cascade); 


        }




        public DbSet<ClothesShopProject.Models.Cart>? Cart { get; set; }

        public DbSet<ClothesShopProject.Models.CartItem>? CartItem { get; set; }


    }

}
