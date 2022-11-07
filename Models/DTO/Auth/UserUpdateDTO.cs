using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class UserUpdateDTO
    {
        [Required]
        public string FullName { get; set; }
        //[Required]
        //public string UserName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
