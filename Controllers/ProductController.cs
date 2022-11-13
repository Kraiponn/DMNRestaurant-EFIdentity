using DMNRestaurant.Data;
using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Product;
using DMNRestaurant.Services;
using Microsoft.AspNetCore.Mvc;

namespace DMNRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /****************************************************************************
         * @Description     Get many Products
         * @Route           GET api/product?skey=[]&page=[]&pageSize=[]
         * @Access          Public
         ***************************************************************************/
        [HttpGet]
        public ActionResult<ResponseAPI<ProductWithPaginateDTO>> GetCategories()
        {
            var respAPI = new ResponseAPI<ProductWithPaginateDTO>();

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
         * @Description     Get a product by id
         * @Route           GET api/product/{pId}
         * @Access          Public
         ***************************************************************************/
        [HttpGet("{pId}")]
        public ActionResult<ResponseAPI<ProductDTO>> GetProductById(string pId)
        {
            var respAPI = new ResponseAPI<ProductDTO>();

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
         * @Description     Add new product
         * @Route           POST api/product
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpPost]
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> CreateProduct([FromForm] ProductCUDTO dto, IFormFile? file)
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
         * @Description     Edit a product by id
         * @Route           PUT api/product
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpPut("{pId}")]
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> UpdateProduct(string pId, [FromForm] ProductCUDTO dto, IFormFile? file)
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
         * @Description     Remove a product by id
         * @Route           DELETE api/product
         * @Access          Private - Admin Account
         ***************************************************************************/
        [HttpDelete("{pId}")]
        public ActionResult<ResponseAPI<HttpMessageResponseDTO>> DeleteProduct(string pId)
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
