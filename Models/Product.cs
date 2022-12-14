namespace DMNRestaurant.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UnitPrice { get; set; }
        public int InStock { get; set; }
        public string Photo { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
