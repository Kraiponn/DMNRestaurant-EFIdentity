using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace DMNRestaurant.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]/manage-account")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAdminRepository _adminRepo;

        public AdminController(IMapper mapper, IAdminRepository adminRepo)
        {
            _mapper = mapper;
            _adminRepo = adminRepo;
        }

        /****************************************************************************
         * @Description     Get many users
         * @Route           GET api/admin/manage-account?skey=[]&page=[]&pageSize=[]
         * @Access          Private(Owner Account)
         ***************************************************************************/
        [HttpGet]
        public async Task<ActionResult<ResponseAPI<List<UsersWithPaginationDTO>>>> GetUsers(string? skey = null, int page = 1, int pageSize = 10)
        {
            var respAPI = new ResponseAPI<List<UsersWithPaginationDTO>>();

            try
            {
                Expression<Func<User, bool>>? filter = !String.IsNullOrEmpty(skey)
                                                        ? (u => u.Email.Contains(skey))
                                                        : null;

                var result = await _adminRepo.GetUsersAsync(
                        Request.Scheme,
                        Request.Host.ToString(),
                        filter,
                        page,
                        pageSize
                    );

                if (result.statusCode == 404)
                {
                    respAPI.ErrorMessage = result.errMessage;
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(respAPI);
                }
                else
                {
                    respAPI.Data = result.userDTOs;
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.IsSuccess = false;
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.ErrorMessage = new List<string> { ex.Message };

                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Get a user
         * @Route           GET api/admin/mange-account/{userId}
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpGet("{userId}")]
        public async Task<ActionResult<ResponseAPI<UserRolesDTO>>> GetUser(string userId)
        {
            var respAPI = new ResponseAPI<UserRolesDTO>();

            try
            {
                var result = await _adminRepo.GetSingleUserAsync(userId);

                if (result.statusCode == 404)
                {
                    respAPI.ErrorMessage = result.errMessage;
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(respAPI);
                }
                else
                {
                    respAPI.Data = result.userRolesDTO;
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.IsSuccess = false;
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.ErrorMessage = new List<string> { ex.Message };

                return BadRequest(respAPI);
            }
        }
    }
}
