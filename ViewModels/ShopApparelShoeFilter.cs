using Microsoft.AspNetCore.Mvc.Rendering;
using ClothesShopProject.Models;

namespace ClothesShopProject.ViewModels
{
    public class ShopApparelShoeFilter
    {
        public IList<ShopApparelShoe> ShopApparelShoes { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } 
        public string Shop { get; set; }
        public Apparel? Apparel { get; set; }
        public Shoe? Shoe { get; set; }
        public SelectList Apparels { get; set; }
        public SelectList Shoes { get; set; }
        public SelectList Shops { get; set; }
        public SelectList SortTypes { get; set; }
        public string Sort { get; set; }

        public SelectList TypeOptions { get; set; }
        public SelectList SortOptions { get; set; }

        public string ApparelSearch { get; set; }
        public string ShoeSearch { get; set; }

        public int Id { get; set; }

        public string Color { get; set; }
        public SelectList ColorOptions { get; set; }
    }
}
