using AutoMapper;
using DMNRestaurant.Data;
using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO.Category;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CategoryRepository> logger;

        public CategoryRepository(IMapper mapper, ApplicationDbContext dbContext, ILogger<CategoryRepository> logger)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            this.logger = logger;
        }

        /************************************************************************
         *                     Checking The Category Exist
         ***********************************************************************/
        public async Task<Category> CategoryExistAsync(string catId)
        {
            return await _dbContext.Categories.FirstAsync(c => c.Id == catId);
        }

        /************************************************************************
         *                        Create New Category
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> CreateAsync(CategoryCUDTO dto)
        {
            int statusCode = 201;
            var errMessage = new List<string>();

            var category = _mapper.Map<Category>(dto);
            category.Id = Guid.NewGuid().ToString();

            await _dbContext.AddAsync(category);
            var result = await _dbContext.SaveChangesAsync();

            if (result != 1)
            {
                statusCode = 400;
                errMessage.Add("Invalid add new Category");
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                        Update a Category
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> UpdateAsync(string catId, CategoryCUDTO dto)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == catId);
            if (category == null)
            {
                statusCode = 404;
                errMessage.Add("Category not found");
                return (statusCode, errMessage);
            }

            category.Name = dto.Name;
            category.Description = dto.Description;
            var result = await _dbContext.SaveChangesAsync();
            //this.logger.LogWarning(">>>>>  Bebug For Update  <<<<<<");
            //this.logger.LogWarning(result.ToString());

            if (result != 1)
            {
                statusCode = 400;
                errMessage.Add("Invalid update category");
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                          Delete a Category
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> DeleteAsync(string catId)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == catId);
            if (category == null)
            {
                statusCode = 404;
                errMessage.Add("Category not found");
                return (statusCode, errMessage);
            }

            _dbContext.Categories.Remove(category);
            var result = await _dbContext.SaveChangesAsync();
            //this.logger.LogWarning(">>>>>  Bebug For Delete  <<<<<<");
            //this.logger.LogWarning(result.ToString());

            if (result != 1)
            {
                statusCode = 400;
                errMessage.Add("Invalid delete category");
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                        Finding Many Categories
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage, CategoryWithPaginateDTO data)>
            FindCategoriesAsync(
                Expression<Func<Category, bool>>? filter,
                int page,
                int pageSize)
        {
            int statusCode = 200;
            var errMessage = new List<string>();
            var data = new CategoryWithPaginateDTO();
            int total = 0;

            var query = filter == null
                            ? _dbContext.Categories.OrderBy(o => o.Name)
                            : _dbContext.Categories.OrderBy(o => o.Name).Where(filter);

            if (query == null)
            {
                statusCode = 404;
                errMessage.Add("Product not found");
                return (statusCode, errMessage, data);
            }
            else
            {
                total = await query.CountAsync();
                var results = await query
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

                if (results == null || results.Count == 0)
                {
                    statusCode = 404;
                    errMessage.Add("Product not found");
                    return (statusCode, errMessage, data);
                }
                else
                {
                    var categories = new List<CategoryDTO>();
                    foreach (var category in results)
                    {
                        categories.Add(_mapper.Map<CategoryDTO>(category));
                    }

                    data = new CategoryWithPaginateDTO
                    {
                        Categories = categories,
                        Total = total,
                        Page = page,
                        PageSize = pageSize
                    };

                    return (statusCode, errMessage, data);
                }
            }
        }

        /************************************************************************
         *                         Finding a Category
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage, CategoryDTO category)>
            FindCategoryByIdAsync(
                string catId,
                bool tracked = true)
        {
            int statusCode = 200;
            var errMessage = new List<string>();
            var category = new CategoryDTO();

            var result = tracked
                            ? await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == catId)
                            : await _dbContext.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == catId);

            if (result == null)
            {
                statusCode = 404;
                errMessage.Add("Category not found");
                return (statusCode, errMessage, category);
            }

            category = _mapper.Map<CategoryDTO>(result);
            return (statusCode, errMessage, category);
        }

    }
}
