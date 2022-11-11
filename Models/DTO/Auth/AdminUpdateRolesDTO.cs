using Microsoft.Build.Framework;

namespace DMNRestaurant.Models.DTO.Auth
{
    public class AdminUpdateRolesDTO
    {
        [Required]
        public string Roles { get; set; }
    }
}
