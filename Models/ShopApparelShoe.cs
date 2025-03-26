using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothesShopProject.Models
{
    public class ShopApparelShoe
    {
        public long Id { get; set; }

        public int ShopId { get; set; }
        public Shop? Shop { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? Color { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }

        public int? ApparelId { get; set; }
        public Apparel? Apparel { get; set; }

        public int? ShoeId { get; set; }
        public Shoe? Shoe { get; set; }
    }
}
