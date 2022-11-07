using DMNRestaurant.Models.DTO.Auth;

namespace DMNRestaurant.Models.DTO
{
    public class HttpAuthResponseDTO
    {
        public string AccessToken { get; set; }
        public UserRolesDTO UserRoles { get; set; }
    }
}
