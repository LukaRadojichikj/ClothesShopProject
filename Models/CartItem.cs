using System.ComponentModel.DataAnnotations;

namespace ClothesShopProject.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Display(Name = "Cart")]
        public int? CartId { get; set; }
        [Display(Name = "Cart")]
        public Cart? Cart { get; set; }

        [Display(Name = "Buy")]
        public long? ShopApparelShoeId { get; set; }
        [Display(Name = "Buy")]
        public ShopApparelShoe? ShopApparelShoe { get; set; }

        public int Quantity { get; set; }

        [Display(Name = "Total Price")]
        public decimal getTotalPricePerItem
        {
            get
            {
                return Quantity * ShopApparelShoe.Price;
            }
        }
    }
}
