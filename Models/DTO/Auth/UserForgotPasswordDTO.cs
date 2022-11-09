using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class UserForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
