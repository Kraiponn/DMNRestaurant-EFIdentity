using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class AdminUpdatePasswordDTO
    {
        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Password length must be between {0} and {1} characters")]
        public string Password { get; set; }
    }
}
