
using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Product
{
    public class ProductCUDTO
    {
        [Required(ErrorMessage = "Product name field is require")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal UnitPrice { get; set; }
        public int InStock { get; set; }

        public string Photo { get; set; }

        [Required]
        public string CategoryId { get; set; }
    }
}
