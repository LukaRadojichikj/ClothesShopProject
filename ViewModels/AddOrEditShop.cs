using System.ComponentModel.DataAnnotations;
using ClothesShopProject.Models;

namespace ClothesShopProject.ViewModels
{
    public class AddOrEditShop
    {
        public Shop? Shop { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? LogoFile { get; set; }

        [Display(Name = "Picture name")]
        public string? LogoName { get; set; }
    }
}
