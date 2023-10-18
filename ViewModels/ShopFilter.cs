using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothesShopProject.ViewModels
{
    public class ShopFilter
    {
        public IList<Shop> Shops { get; set; }
        public string Name { get; set; }

        public string Country { get; set; }
        public string NumOfShops { get; set; }
        public string Logo { get; set; }
    }
}
