namespace DMNRestaurant.Models.DTO.Category
{
    public class CategoryWithPaginateDTO
    {
        public ICollection<CategoryDTO> Categories { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
    }
}
