using System.ComponentModel.DataAnnotations;
using ClothesShopProject.Models;

namespace ClothesShopProject.ViewModels
{
    public class AddOrEditShoe
    {
        public Shoe? Shoe { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? PictureFile { get; set; }

        [Display(Name = "Picture name")]
        public string? PictureName { get; set; }
    }
}
