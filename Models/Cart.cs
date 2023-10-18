using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClothesShopProject.Areas.Identity.Data;

namespace ClothesShopProject.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Display(Name = "User Email")]
        public string? ClothesShopProjectId { get; set; }
        [ForeignKey("ClothesShopProjectId")]
        [Display(Name = "User Email")]
        public ClothesShopProjectUser? ClothesShopProjectUser { get; set; }

        public ICollection<CartItem>? CartItems { get; set; }
    }
}
