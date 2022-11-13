using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Category
{
    public class CategoryCUDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
