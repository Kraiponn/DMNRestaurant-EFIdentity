using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Helper.Enum;
using DMNRestaurant.Models.DTO;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Services;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

namespace DMNRestaurant.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public AccountsController(
            ILogger<AccountsController> logger,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _logger = logger;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        /*********************************************************************************
         * @Description     Get many accounts
         * @Route           GET api/accounts?page=[Number]&pageSize=[Number]&searchKey=[?]
         * @Access          Private(Admin)
         ********************************************************************************/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers(string? searchKey = null, int page = 1, int pageSize = 10)
        {
            var respAPI = new ResponseAPI<IEnumerable<UsersWithPaginationDTO>>();
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

                // Update full url to photo field
                foreach (var user in response)
                {
                    //
                }

                respAPI.Data = _mapper.Map<IEnumerable<UsersWithPaginationDTO>>(response);
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
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var respAPI = new ResponseAPI<UserRolesDTO>();

            try
            {
                //string userId = User.Claims.First(c => c.Type == "userId").Value;
                //if (User.Claims == null)
                //{
                //    return Unauthorized();
                //}
                var accessToken = await HttpContext.GetTokenAsync(EJwtAliasKey.access_token.ToString());
                var payload = new JwtSecurityToken(accessToken);
                var userId = payload.Claims.First(p => p.Type == EJwtAliasKey.user_id.ToString()).Value;

                (int responseCode, List<string> messages, UserRolesDTO responseData) =
                            await _userRepo.GetSingleUserAsync(userId);

                if (responseCode == 0)
                {
                    respAPI.StatusCode = System.Net.HttpStatusCode.NotFound;
                    respAPI.IsSuccess = false;
                    respAPI.ErrorMessage = messages;
                    return NotFound(respAPI);
                }

                // Update full url to photo field
                responseData.Photo = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/images/accounts/{responseData.Photo}";

                respAPI.Data = _mapper.Map<UserRolesDTO>(responseData);
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

        /********************************************************************
         * @Description     Register new a account
         * @Route           POST api/accounts/signup
         * @Access          Public
         *******************************************************************/
        [HttpPost("singup")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseAPI<List<string>>>> Signup(UserCreateDTO userCreateDTO)
        {
            var respAPI = new ResponseAPI<List<string>>();

            try
            {
                var userExists = await _userRepo.UserExists(userCreateDTO.Email);

                if (userExists != null)
                {
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.ErrorMessage = new List<string> { "An email already exists" };
                    return BadRequest(respAPI);
                }
                else
                {
                    //User user = _mapper.Map<User>(userCreateDTO);
                    var user = new User
                    {
                        UserName = userCreateDTO.Email,
                        Email = userCreateDTO.Email,
                    };

                    (int responseCode, List<string> messages, UserRolesDTO responseData) =
                               await _userRepo.SignupAsync(user, userCreateDTO.Password);

                    if (responseCode == 0)
                    {
                        respAPI.IsSuccess = false;
                        respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        respAPI.ErrorMessage = messages;
                        return BadRequest(respAPI);
                    }

                    respAPI.StatusCode = System.Net.HttpStatusCode.Created;
                    respAPI.Data = new List<string> { "Register is successfully" };
                    return Created("", respAPI);
                }
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
         * @Access          Public
         *******************************************************************/
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var respAPI = new ResponseAPI<UserRolesDTO>();

            try
            {
                (int responseCode, List<string> messages, UserRolesDTO responseData) =
                        await _userRepo.SigninAsync(loginDTO);

                if (responseCode == 0)
                {
                    respAPI.IsSuccess = false;
                    respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    respAPI.ErrorMessage = messages;
                    return BadRequest(respAPI);
                }

                respAPI.Data = responseData;
                return Ok(respAPI);
            }
            catch (Exception ex)
            {
                respAPI.IsSuccess = false;
                respAPI.StatusCode = System.Net.HttpStatusCode.BadRequest;
                respAPI.ErrorMessage = new List<string> { ex.Message };
                return BadRequest(respAPI);
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
                    responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    responseApi.ErrorMessage = messages;
                    return BadRequest(responseApi);
                }

                responseApi.ErrorMessage = messages;
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.IsSuccess = false;
                responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                responseApi.ErrorMessage = new List<string>() { ex.Message };
                return BadRequest(responseApi);
            }
        }

        /********************************************************************
         * @Description     Forgot password
         * @Route           POST api/accounts/forgot-password
         * @Access          Public
         *******************************************************************/
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<UserForgotPasswordResponseDTO>> ForgotPassword(UserForgotPasswordDTO dto)
        {
            var responseApi = new ResponseAPI<UserForgotPasswordResponseDTO>();

            try
            {
                var user = await _userRepo.UserExists(dto.Email);
                if (user == null)
                {
                    responseApi.IsSuccess = false;
                    responseApi.StatusCode = System.Net.HttpStatusCode.NotFound;
                    responseApi.ErrorMessage = new List<string> { "Account not found" };
                    return NotFound(responseApi);
                }

                var result = await _userRepo.ForgotPasswordAsync(user);
                if (result.responseCode == 0)
                {
                    responseApi.IsSuccess = false;
                    responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    responseApi.ErrorMessage = result.message;
                    return BadRequest(responseApi);
                }

                responseApi.Data = new UserForgotPasswordResponseDTO
                {
                    ResetToken = result.resetToken
                };

                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.IsSuccess = false;
                responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                responseApi.ErrorMessage = new List<string>() { ex.Message };
                return BadRequest(responseApi);
            }
        }

        /********************************************************************
         * @Description     Reset password
         * @Route           POST api/accounts/reset-password
         * @Access          Public
         *******************************************************************/
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(UserResetPasswordDTO dto)
        {
            var responseApi = new ResponseAPI<HttpMessageResponseDTO>();

            try
            {
                var user = await _userRepo.UserExists(dto.Email);
                if (user == null)
                {
                    responseApi.IsSuccess = false;
                    responseApi.StatusCode = System.Net.HttpStatusCode.NotFound;
                    responseApi.ErrorMessage = new List<string> { "Account not found" };
                    return NotFound(responseApi);
                }

                var result = await _userRepo.ResetPasswordAsync(
                    user, dto.ResetToken, dto.Password);

                if (result.responseCode == 0)
                {
                    responseApi.IsSuccess = false;
                    responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    responseApi.ErrorMessage = result.messages;
                    return BadRequest(responseApi);
                }

                responseApi.Data = new HttpMessageResponseDTO { Message = "Your password has been reset." };
                return Ok(responseApi);
            }
            catch (Exception ex)
            {
                responseApi.IsSuccess = false;
                responseApi.StatusCode = System.Net.HttpStatusCode.BadRequest;
                responseApi.ErrorMessage = new List<string>() { ex.Message };
                return BadRequest(responseApi);
            }
        }

    }
}
