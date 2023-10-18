using System.ComponentModel.DataAnnotations;
using ClothesShopProject.Models;

namespace ClothesShopProject.ViewModels
{
    public class AddOrEditBrand
    {
        public Brand? Brand { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? ProfilePictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? ProfilePictureName { get; set; }
    }
}
