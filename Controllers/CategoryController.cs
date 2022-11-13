using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Category;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

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
         * @Route           GET api/category?skey=[]&page=[]&pageSize=[]
         * @Access          Public
         ***************************************************************************/
        [HttpGet]
        public ActionResult<List<ResponseAPI<CategoryDTO>>> GetCategories()
        {
            var respAPI = new ResponseAPI<List<CategoryDTO>>();

            try
            {
                //
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
        public ActionResult<ResponseAPI<CategoryDTO>> GetCategoryById(string catId)
        {
            var respAPI = new ResponseAPI<CategoryDTO>();

            try
            {
                //
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
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> CreateCategory(CategoryCUDTO dto)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                //
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
         * @Description     Edit a category by id
         * @Route           PUT api/category
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpPut("{catId}")]
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> UpdateCategory(string catId, CategoryCUDTO dto)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                //
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
         * @Description     Remove a category by id
         * @Route           DELETE api/category
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpDelete("{catId}")]
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> DeleteCategory(string catId)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                //
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

    }
}
