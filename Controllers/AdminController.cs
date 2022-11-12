using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace DMNRestaurant.Controllers
{
    [Authorize(Roles = "Admin")]
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
                var result = await _adminRepo.GetSingleUserAsync(
                    userId,
                    Request.Scheme,
                    Request.Host.ToString()
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

        /****************************************************************************
         * @Description     Create a new member
         * @Route           POST api/admin/mange-account/create
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpPost("create")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> CreateUser(UserCreateDTO dTO)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var userExist = await _adminRepo.UserExistAsync(dTO.Email);
                if (userExist != null)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = new List<string> { "An email address already exists" };
                    return Conflict(respAPI);
                }

                var user = _mapper.Map<User>(dTO);
                var result = await _adminRepo.CreateAsync(user);

                if (result.statusCode != 201)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = new List<string> { "Invalid create account" };
                }
                else
                {
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Account created successfully" };
                    respAPI.StatusCode = System.Net.HttpStatusCode.Created;
                }


                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message.ToString() };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Update user
         * @Route           PUT api/admin/mange-account/{userId}/account
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpPut("{userId}/account")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> UpdateUser(string userId, [FromForm] UserUpdateDTO dTO, IFormFile? file)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _adminRepo.UpdateAsync(userId, dTO, file);

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
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Account updated successfully" };
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message.ToString() };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Update password of user
         * @Route           PUT api/admin/mange-account/{userId}/password
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpPut("{userId}/password")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> UpdatePassword(string userId, AdminUpdatePasswordDTO dTO)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _adminRepo.UpdatePasswordAsync(userId, dTO);

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
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Password updated successfully" };
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message.ToString() };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Update roles of user
         * @Route           PUT api/admin/mange-account/{userId}/roles
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpPut("{userId}/roles")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> UpdateRoles(string userId, AdminUpdateRolesDTO dTO)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _adminRepo.UpdateRolesAsync(userId, dTO);

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
                    respAPI.Data = new HttpMessageResponseDTO { Message = $"Account id {userId} has been update to new roles" };
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message.ToString() };
                return BadRequest(respAPI);
            }
        }

        /****************************************************************************
         * @Description     Delete account by user id
         * @Route           DELETE api/admin/mange-account/{userId}/delete
         * @Access          Private(Admin Account)
         ***************************************************************************/
        [HttpDelete("{userId}/delete")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> DeleteUser(string userId)
        {
            var respAPI = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var results = await _adminRepo.DeleteAsync(userId);

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
                    respAPI.Data = new HttpMessageResponseDTO { Message = "Account has been delete." };
                    return Ok(respAPI);
                }
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message.ToString() };
                return BadRequest(respAPI);
            }
        }
    }
}
