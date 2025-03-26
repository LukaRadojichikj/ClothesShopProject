namespace ClothesShopProject.Models
{
    public class Shoe
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Colour { get; set; }
        public int Size { get; set; }
        public string? Picture { get; set; }


        public ICollection<ShopApparelShoe>? ShopApparelShoes { get; set; }
    }
}
