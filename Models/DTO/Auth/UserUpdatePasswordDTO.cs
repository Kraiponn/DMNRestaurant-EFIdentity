using System.ComponentModel.DataAnnotations;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class UserUpdatePasswordDTO
    {
        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Current Password length must be between {0} and {1} characters")]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 6, ErrorMessage = " New Password length must be between {0} and {1} characters")]
        public string NewPassword { get; set; }
    }
}
