using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesShopProject.Models
{
    public class ShopApparelShoe
    {
        public long Id { get; set; }

        public int ShopId { get; set; }
        public Shop? Shops { get; set; }

        // Common properties for both Apparel and Shoe
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
        public string? Color { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        // Specific properties for Apparel
        public int? ApparelId { get; set; }
        public Apparel? Apparel { get; set; }

        // Specific properties for Shoe
        public int? ShoeId { get; set; }
        public Shoe? Shoe { get; set; }
    }
}
