using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq.Expressions;

namespace DMNRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<User> uManager;

        public AccountsController(IUserRepository userRepo, IMapper mapper, UserManager<User> uManager)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            this.uManager = uManager;
        }

        /*********************************************************************************
         * @Description     Get many accounts
         * @Route           GET api/accounts?page=[Number]&pageSize=[Number]&searchKey=[?]
         * @Access          Private(Admin)
         ********************************************************************************/
        [HttpGet]
        public async Task<IActionResult> GetUsers(string? searchKey = null, int page = 1, int pageSize = 10)
        {
            var respAPI = new ResponseAPI<IEnumerable<UserDTO>>();
            Expression<Func<User, bool>>? filter = !string.IsNullOrEmpty(searchKey)
                    ? (k => k.Email.StartsWith(searchKey))
                    : null;
            try
            {
                var response = await _userRepo.GetUsersAsync(filter, page, pageSize, true);
                if (response == null)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = new List<string> { "Account Not Found" };
                    return NotFound(respAPI);
                }

                respAPI.Data = _mapper.Map<IEnumerable<UserDTO>>(response);
                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /********************************************************************
         * @Description     Get a profile
         * @Route           GET api/accounts/profile/{id}
         * @Access          Private(Owner Account)
         *******************************************************************/
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            var respAPI = new ResponseAPI<UserRolesDTO>();

            try
            {
                (int responseCode, List<string> messages, UserRolesDTO responseData) =
                            await _userRepo.GetSingleUserAsync(id);

                if (responseCode == 0)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = messages;
                    return NotFound(respAPI);
                }

                respAPI.Data = _mapper.Map<UserRolesDTO>(responseData);
                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                respAPI.IsSuccess = false;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
            }
        }

        /********************************************************************
         * @Description     Register new a member
         * @Route           POST api/memebers
         * @Access          Private(Admin)
         *******************************************************************/
        [HttpPost("singup")]
        public async Task<ActionResult<ResponseAPI<List<string>>>> Signup([FromBody] UserCreateDTO userDTO)
        {
            var respAPI = new ResponseAPI<List<string>>();
            User user = _mapper.Map<User>(userDTO);

            try
            {
                if (await _userRepo.UserExists(user))
                {
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.ErrorMessage = new List<string> { "An email already exists" };
                    return BadRequest(respAPI);
                }

                (int responseCode, List<string> messages, UserRolesDTO responseData) =
                               await _userRepo.SignupAsync(user, userDTO.Password);

                if (responseCode == 0)
                {
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.ErrorMessage = messages;
                    return BadRequest(respAPI);
                }

                respAPI.StatusCode = System.Net.HttpStatusCode.Created;
                respAPI.Data = new List<string> { "New member created is successfully" };
                return CreatedAtRoute("", respAPI);
            }
            catch (DBConcurrencyException errors)
            {
                respAPI.IsSuccess = false;
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.ErrorMessage = new List<string> { errors.Message };
                return BadRequest(respAPI);
            }
        }

        /********************************************************************
         * @Description     Login 
         * @Route           POST api/accounts/signin
         * @Access          Private(Admin)
         *******************************************************************/
        [HttpPost("signin")]
        public async Task<ActionResult> Login()
        {
            try
            {
                //
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /********************************************************************
         * @Description     Editting an account
         * @Route           PUT api/accounts/update-account/{id}
         * @Access          Private(Owner account | Admin)
         *******************************************************************/
        [HttpPut("update-account/{id}")]
        public async Task<ActionResult<ResponseAPI<List<string>>>> UpdateAccount(string id, [FromForm] UserUpdateDTO updateDTO, IFormFile file)
        {
            var responseApi = new ResponseAPI<string>();

            try
            {
                //var token = await HttpContext.GetTokenAsync("access_token");
                //var payload = new JwtSecurityToken(token);
                //var userId = payload.Claims.First(p => p.Type == "userId").Value;

                //var user = _mapper.Map<User>(dto);
                (int responseCode, List<string> messages) =
                        await _userRepo.UpdateAsync(id, updateDTO, file);

                if (responseCode == 0)
                {
                    responseApi.IsSuccess = false;
                    responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    responseApi.ErrorMessage = messages;
                    return BadRequest(responseApi);
                }
                else
                {
                    responseApi.Data = "Account updated successfully";
                    return Ok(responseApi);
                }
            }
            catch (Exception dbEx)
            {
                responseApi.IsSuccess = false;
                responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                responseApi.ErrorMessage = new List<string> { dbEx.ToString() };
                return BadRequest(responseApi);
            }
        }

        /********************************************************************
         * @Description     Delete an account
         * @Route           DELETE api/accounts/delete-account/{id}
         * @Access          Private(Owner account | Admin)
         *******************************************************************/
        [HttpDelete("delete-account/{id}")]
        public async Task<ActionResult<ResponseAPI<HttpMessageResponseDTO>>> DeleteAccount(string id)
        {
            var responseApi = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                (int responseCode, List<string> messages) = await _userRepo.DeleteAsync(id);
                if (responseCode == 0)
                {
                    responseApi.IsSuccess = false;
                    responseApi.ErrorMessage = messages;
                    return BadRequest(responseApi);
                }

                responseApi.ErrorMessage = messages;
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.IsSuccess = false;
                responseApi.ErrorMessage = new List<string>() { ex.Message };
                return BadRequest(responseApi);
            }

        }
    }
}
