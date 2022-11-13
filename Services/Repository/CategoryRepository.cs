using AutoMapper;
using DMNRestaurant.Data;
using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Category;
using DMNRestaurant.Services.Repository.IRepository;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(IMapper mapper, ApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /************************************************************************
         *                     Checking The Category Exist
         ***********************************************************************/
        public Task<Category> CategoryExistAsync(string catId)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Create New Category
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> CreateAsync(CategoryCUDTO dto)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Update a Category
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> UpdateAsync(string catId, CategoryCUDTO dto)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                          Delete a Category
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage)> DeleteAsync(string catId)
        {
            throw new NotImplementedException();
        }

        /************************************************************************
         *                        Finding Many Categories
         ***********************************************************************/
        public Task<(int statusCode, List<string> errMessage, List<CategoryWithPaginateDTO> categories)>
            FindCategoriesAsync(
                Expression<Func<Category, bool>>? filter,
                int page,
                int pageSize,
                bool tracked = true)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var query = _dbContext.Categories;

            throw new NotImplementedException();

            //return (statusCode, errMessage, categories);
        }

        /************************************************************************
         *                         Finding a Category
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage, CategoryDTO category)>
            FindCategoryByIdAsync(string catId)
        {
            throw new NotImplementedException();
        }

    }
}
