namespace DMNRestaurant.Models.DTO.Auth
{
    public class UserRolesDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public IList<string> Roles { get; set; }
        public string? AccessToken { get; set; }
    }
}
