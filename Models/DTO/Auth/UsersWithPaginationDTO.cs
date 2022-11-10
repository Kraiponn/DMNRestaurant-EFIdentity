namespace DMNRestaurant.Models.DTO.Auth
{
    public class UsersWithPaginationDTO
    {
        public List<UserDTO> Users { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
    }
}
