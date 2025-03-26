using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ClothesShopProject.ViewModels
{
    public class ApparelFilter
    {
        public IList<Apparel> Apparels { get; set; }

        [Display(Name = "Color")]
        public string Colour { get; set; }
        public SelectList Colours { get; set; }

        public string Name { get; set; }
        public string Picture { get; set; }

        [Display(Name = "Brand")]
        public int? BrandId { get; set; } 
        public SelectList Brands { get; set; } 
    }
}
