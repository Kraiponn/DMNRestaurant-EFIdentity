namespace DMNRestaurant.Models.DTO.Product
{
    public class ProductWithPaginateDTO
    {
        public ICollection<ProductDTO> Products { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
    }
}
