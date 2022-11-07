using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO.Auth;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository.IRepository
{
    // where T : class
    // Expresssion<Func<T,bool>>?
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync(
            Expression<Func<User, bool>>? filter = null,
            int page = 1,
            int pageSize = 10,
            bool tracked = true);
        Task<(int responseCode, List<string> messages, UserRolesDTO responseData)> GetSingleUserAsync(string userId);

        Task<(int responseCode, List<string> messages, UserRolesDTO responseData)> SignupAsync(User user, string password);
        Task<(int responseCode, List<string> messages, UserRolesDTO responseData)> SigninAsync(string email, string password);
        Task<(int responseCode, List<string> messages)> UpdateAsync(string userId, UserUpdateDTO userUpdateDTO, IFormFile file);
        Task<(int responseCode, List<string> messages)> DeleteAsync(string userId);

        Task<bool> UserExists(User user);
        Task<UserRolesDTO> GetUserRolesAsync(string email);

    }
}
