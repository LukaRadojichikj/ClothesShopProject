using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothesShopProject.ViewModels
{
    public class AddOrEditApparel
    {
        public Apparel? Apparel { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? PictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? PictureName { get; set; }

        [Required]
        [Display(Name = "Select Shop")]
        public int? ShopId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public List<SelectListItem>? AvailableShops { get; set; }
    }
}
