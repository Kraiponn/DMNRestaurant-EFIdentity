using DMNRestaurant.Models;
using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Category;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace DMNRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _dbService;

        public CategoryController(ICategoryRepository dbService)
        {
            _dbService = dbService;
        }

        /****************************************************************************
         * @Description     Get many categories
         * @Route           GET api/category?skey=[string]&page=[Number]&pageSize=[Number]
         * @Access          Public
         ***************************************************************************/
        [HttpGet]
        public async Task<ActionResult<CategoryWithPaginateDTO>> GetCategories(string? skey, int page = 1, int pageSize = 10)
        {
            var respAPI = new ResponseAPI<CategoryWithPaginateDTO>();

            try
            {
                Expression<Func<Category, bool>>? filter = skey != null
                                                            ? (p => p.Name.Contains(skey))
                                                            : null;

                var results = await _dbService.FindCategoriesAsync(filter, page, pageSize);
                if (results.statusCode == 404)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return NotFound(respAPI);
                }

                respAPI.Data = results.data;
                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Get a category by id
         * @Route           GET api/category/{catId}
         * @Access          Public
         ***************************************************************************/
        [HttpGet("{catId}")]
        public async Task<ActionResult<ResponseAPI<CategoryDTO>>> GetCategoryById(string catId)
        {
            var respAPI = new ResponseAPI<CategoryDTO>();

            try
            {
                var results = await _dbService.FindCategoryByIdAsync(catId, true);

                if (results.statusCode == 404)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return NotFound(respAPI);
                }

                respAPI.Data = results.category;
                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Add new category
         * @Route           POST api/category
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpPost]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> CreateCategory(CategoryCUDTO dto)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _dbService.CreateAsync(dto);

                if (results.statusCode != 201)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return BadRequest(respAPI);
                }

                respAPI.Data = new HttpMessageResponseDTO { Message = "Category created successfully" };
                return Created("CreatedCategory", respAPI);

            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Edit a category by id
         * @Route           PUT api/category
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpPut("{catId}")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> UpdateCategory(string catId, CategoryCUDTO dto)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _dbService.UpdateAsync(catId, dto);

                if (results.statusCode == 400)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return BadRequest(respAPI);
                }
                else if (results.statusCode == 404)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return NotFound(respAPI);
                }
                else
                {
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Category updated successfully" };
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Remove a category by id
         * @Route           DELETE api/category
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpDelete("{catId}")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> DeleteCategory(string catId)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _dbService.DeleteAsync(catId);

                if (results.statusCode == 400)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return BadRequest(respAPI);
                }
                else if (results.statusCode == 404)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = results.errMessage;
                    return NotFound(respAPI);
                }
                else
                {
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Category deleted successfully" };
                    return Ok(respAPI);
                }

            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

    }
}
