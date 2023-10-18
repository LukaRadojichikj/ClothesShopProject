using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothesShopProject.ViewModels
{
    public class BrandFilter
    {
        public IList<Brand> Brands { get; set; }

        public string Name { get; set; }

        public SelectList Shops { get; set; }

        public string Shop { get; set; }
    }
}
