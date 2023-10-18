using System.ComponentModel.DataAnnotations;

namespace ClothesShopProject.Models
{
    public class Shop
    {
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string Country { get; set; }
        [Display(Name="Number of Shops")]
        public int NumOfShops { get; set; }
        public string? Logo { get; set; }

        public ICollection<Brand>? Brands { get; set; }

        public ICollection<ShopApparelShoe>? ShopApparelShoes { get; set; }

    }
}
