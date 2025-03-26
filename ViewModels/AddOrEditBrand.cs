using System.ComponentModel.DataAnnotations;
using ClothesShopProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothesShopProject.ViewModels
{
    public class AddOrEditBrand
    {
        public Brand Brand { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? ProfilePictureName { get; set; }

        [Required]
        [Display(Name = "Select Shop")]
        public int? ShopId { get; set; }

        public List<SelectListItem>? AvailableShops { get; set; }
    }
}
