using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Category;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<(int statusCode, List<string> errMessage, CategoryWithPaginateDTO data)> FindCategoriesAsync(
                Expression<Func<Category, bool>>? filter,
                int page,
                int pageSize
            );
        Task<(int statusCode, List<string> errMessage, CategoryDTO category)> FindCategoryByIdAsync(
                string catId,
                bool tracked = true);
        Task<Category> CategoryExistAsync(string catId);
        Task<(int statusCode, List<string> errMessage)> CreateAsync(CategoryCUDTO dto);
        Task<(int statusCode, List<string> errMessage)> UpdateAsync(string catId, CategoryCUDTO dto);
        Task<(int statusCode, List<string> errMessage)> DeleteAsync(string catId);
    }
}
