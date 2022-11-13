using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Product;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<(int statusCode, List<string> errMessage, List<ProductWithPaginateDTO> results)> FindPrductsAsync(
                Expression<Func<Product, bool>>? filter,
                int page,
                int pageSize,
                bool tracked = true
            );
        Task<(int statusCode, List<string> errMessage, ProductDTO category)> FindProductByIdAsync(string pId);
        Task<Product> ProductExistAsync(string pId);
        Task<(int statusCode, List<string> errMessage)> CreateAsync(ProductCUDTO dto, IFormFile? file);
        Task<(int statusCode, List<string> errMessage)> UpdateAsync(string pId, ProductCUDTO dto, IFormFile? file);
        Task<(int statusCode, List<string> errMessage)> DeleteAsync(string pId);
    }
}
