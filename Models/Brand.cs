using System.ComponentModel.DataAnnotations;

namespace ClothesShopProject.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Year")]
        [Required]
        public string Year { get; set; }

        [Display(Name = "Country Of Origin")]
        [Required]
        public string CountryOfOrigin { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Picture")]
        public string? ProfilePicture { get; set; }

        [Display(Name = "Shop")]
        public int? ShopId { get; set; }
        public Shop? Shop { get; set; }


    }
}
