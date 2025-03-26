namespace ClothesShopProject.Models
{
    public class Apparel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Colour { get; set; }
        public string? Picture { get; set; }
        public string Type { get; } = "Apparel";

        public ICollection<ShopApparelShoe>? ShopApparelShoes { get; set; }
    }
}
