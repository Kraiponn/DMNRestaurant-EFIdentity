using AutoMapper;
using DMNRestaurant.Data;
using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Product;
using DMNRestaurant.Services.Repository.IRepository;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        /************************************************************************
         *                     Checking The Product Exist
         ***********************************************************************/
        public Task<Product> ProductExistAsync(string pId)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Create New Product
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> CreateAsync(ProductCUDTO dto, IFormFile? file)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Update a Product
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> UpdateAsync(string pId, ProductCUDTO dto, IFormFile? file)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                          Delete a Product
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> DeleteAsync(string pId)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Finding Many Products
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage, List<ProductWithPaginateDTO> results)>
            FindPrductsAsync(
                Expression<Func<Product, bool>>? filter,
                int page,
                int pageSize,
                bool tracked = true)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                         Finding a Product
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage, ProductDTO category)> FindProductByIdAsync(string pId)
        {
            throw new NotImplementedException();
        }
    }
}
