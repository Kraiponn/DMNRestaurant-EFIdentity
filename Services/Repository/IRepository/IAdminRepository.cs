using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO.Auth;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface IAdminRepository
    {
        Task<(int statusCode, List<string> errMessage, List<UsersWithPaginationDTO> userDTOs)> GetUsersAsync(
                string scheme,
                string host,
                Expression<Func<User, bool>>? filter,
                int page,
                int pageSize,
                bool tracked = true
            );
        Task<(int statusCode, List<string> errMessage, UserRolesDTO userRolesDTO)> GetSingleUserAsync(
                string userId,
                string scheme,
                string host
            );
        Task<User> UserExistAsync(string email);
        Task<(int statusCode, List<string> errMessage)> CreateAsync(User user);
        Task<(int statusCode, List<string> errMessage)> UpdateAsync(string userId, UserUpdateDTO dto, IFormFile? file);
        Task<(int statusCode, List<string> errMessage)> UpdatePasswordAsync(string userId, AdminUpdatePasswordDTO dto);
        Task<(int statusCode, List<string> errMessage)> UpdateRolesAsync(string userId, AdminUpdateRolesDTO dto);
        Task<(int statusCode, List<string> errMessage)> DeleteAsync(string userId);
    }
}
