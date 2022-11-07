using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "An email field must be an email type")]
        public string Email { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 5, ErrorMessage = "Password length must be between {1} and {2} characters")]
        public string Password { get; set; }
    }
}
