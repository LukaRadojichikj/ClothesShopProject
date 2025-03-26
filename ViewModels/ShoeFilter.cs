using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClothesShopProject.ViewModels
{
    public class ShoeFilter
    {
        public IList<Shoe> Shoes { get; set; }

        [Display(Name = "Color")]
        public string Colour { get; set; }
        public SelectList Colours { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }

        [Display(Name = "Sort by Size")]
        public string Sort { get; set; }
        public int Size { get; set; }
    }
}
