using AutoMapper;
using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Helper.Enum;
using DMNRestaurant.Models.DTO.Auth;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DMNRestaurant.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> logger;
        private readonly IMapper _mapper;
        private readonly IPhotoRepository _photoRepo;
        private readonly ISecurityRepository _secureRepo;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signinManager;

        public UserRepository(
                ILogger<UserRepository> logger,
                IMapper mapper,
                IPhotoRepository photoRepo,
                ISecurityRepository secureRepo,
                UserManager<User> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<User> signinManager)
        {
            this.logger = logger;
            _mapper = mapper;
            _photoRepo = photoRepo;
            _secureRepo = secureRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _signinManager = signinManager;
        }

        /************************************************************************
         *                        Create New an Account
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages, UserRolesDTO responseData)>
            SignupAsync(User user, string password)
        {
            int responseCode = 1;
            var messages = new List<string>();
            var userRoles = new UserRolesDTO();

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync(ERoles.Member.ToString());
                if (!roleExists)
                {
                    var role = new IdentityRole(ERoles.Member.ToString());
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, ERoles.Member.ToString());
                //await _signinManager.SignInAsync(user, isPersistent: false);
            }
            else
            {
                responseCode = 0;
                foreach (var item in result.Errors)
                {
                    messages.Add(item.Description);
                }
            }

            return (responseCode, messages, userRoles);
        }

        /************************************************************************
         *                        Login to Access Data
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages, UserRolesDTO responseData)>
            SigninAsync(LoginDTO loginDTO)
        {
            int responseCode = 1;
            var messages = new List<string>();
            var userRoles = new UserRolesDTO();

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                responseCode = 0;
                messages.Add("Account Not Found");
                return (responseCode, messages, userRoles);
            }

            var result = await _signinManager.PasswordSignInAsync(
                    loginDTO.Email,
                    loginDTO.Password,
                    false,
                    false
                );

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _secureRepo.GenerateJwtToken(user, roles);

                userRoles = new UserRolesDTO()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.Email,
                    Email = user.Email,
                    Address = user.Address,
                    Photo = user.Photo,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles,
                    AccessToken = accessToken
                };

                messages.Add("Signin is successfully");
                return (responseCode, messages, userRoles);
            }
            else
            {
                responseCode = 0;
                messages.Add("Password is incorrect. Please try again");
                return (responseCode, messages, userRoles);
            }
        }

        /************************************************************************
         *                        Remove New an Account
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages)> DeleteAsync(string userId)
        {
            int responseCode = 1;
            var messages = new List<string>();

            var userExist = await _userManager.FindByIdAsync(userId);
            if (userExist == null)
            {
                responseCode = 0;
                messages.Add("Account Not Found");
            }
            else
            {
                var delResult = await _userManager.DeleteAsync(userExist);
                if (delResult.Succeeded)
                {
                    if (userExist.Photo != "nopic.png")
                    {
                        _photoRepo.Remove(userExist.Photo);
                    }

                    messages.Add("Account deleted successfully");
                }
                else
                {
                    responseCode = 0;
                    foreach (var item in delResult.Errors)
                    {
                        messages.Add(item.Description);
                    }
                }
            }

            return (responseCode, messages);
        }

        /************************************************************************
         *                        Editing an Account
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages)> UpdateAsync(string userId, UserUpdateDTO userUpdateDTO, IFormFile file)
        {
            int responseCode = 1;
            var messages = new List<string>();

            var userExist = await _userManager.FindByIdAsync(userId);
            if (userExist == null)
            {
                responseCode = 0;
                messages.Add("User Not Found");
                return (responseCode, messages);
            };

            if (_photoRepo.IsUpdaload(file))
            {
                if (userExist.Photo != "nopic.png")
                {
                    _photoRepo.Remove(userExist.Photo);
                }

                var result = _photoRepo.Validation(file);
                if (string.IsNullOrEmpty(result))
                {
                    var imageName = await _photoRepo.Upload(file);
                    userExist.Photo = imageName;
                }
            }

            userExist.FullName = userUpdateDTO.FullName;
            userExist.Address = userUpdateDTO.Address;
            userExist.PhoneNumber = userUpdateDTO.PhoneNumber;

            var updateResult = await _userManager.UpdateAsync(userExist);
            if (!updateResult.Succeeded)
            {
                responseCode = 0;
                foreach (var item in updateResult.Errors)
                {
                    messages.Add(item.Description);
                }
            }

            messages.Add("Account updated successfully");
            return (responseCode, messages);
        }

        /************************************************************************
         *                        Get One Account
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages, UserRolesDTO responseData)>
            GetSingleUserAsync(string userId)
        {
            int responseCode = 1;
            var messages = new List<string>();
            var userRoles = new UserRolesDTO();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                responseCode = 0;
                messages.Add("Account Not Found");
                return (responseCode, messages, userRoles);
            }

            var userRolesExists = await this.GetUserRolesAsync(user.Id);
            if (userRolesExists == null)
            {
                responseCode = 0;
                messages.Add("Role Not Found");
                return (responseCode, messages, userRoles);
            }

            userRoles = userRolesExists;
            return (responseCode, messages, userRoles);
        }

        /************************************************************************
         *                      Get User And Role Types
         ***********************************************************************/
        public async Task<UserRolesDTO> GetUserRolesAsync(string userId)
        {
            var userRoles = new UserRolesDTO();

            User? user = await _userManager.FindByIdAsync(userId);
            if (user == null) return userRoles;

            IList<string>? roles = await _userManager.GetRolesAsync(user);
            if (roles == null) return userRoles;

            return new UserRolesDTO()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Photo = user.Photo,
                Roles = roles
            };
        }

        /************************************************************************
         *                          Update Password
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages)> UpdatePasswordAsync(User user, string resetToken, string newPassword)
        {
            int responseCode = 1;
            var messages = new List<string>();

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
            {
                responseCode = 0;
                foreach (var item in result.Errors)
                {
                    messages.Add(item.Description);
                }

                return (responseCode, messages);
            }

            return (responseCode, messages);
        }

        /************************************************************************
         *                          Forgot Password
         ***********************************************************************/
        public async Task<(int responseCode, List<string> message, string resetToken)> ForgotPasswordAsync(User user)
        {
            int responseCode = 1;
            var messages = new List<string>();

            var resetTokenCode = await _userManager.GeneratePasswordResetTokenAsync(user);
            return (responseCode, messages, resetTokenCode);
        }

        /************************************************************************
         *                          Reset Password
         ***********************************************************************/
        public async Task<(int responseCode, List<string> messages)> ResetPasswordAsync(User user, string resetToken, string newPassword)
        {
            int responseCode = 1;
            var messages = new List<string>();

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
            {
                responseCode = 0;
                foreach (var item in result.Errors)
                {
                    messages.Add(item.Description);
                }

                return (responseCode, messages);
            }

            return (responseCode, messages);
        }

        /************************************************************************
         *                        Get Manay Accounts
         ***********************************************************************/
        public async Task<IEnumerable<User>> GetUsersAsync(Expression<Func<User, bool>>? filter = null, int page = 1, int pageSize = 10, bool tracked = true)
        {
            var response = _userManager.Users
                            .OrderBy(o => o.Email)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize);

            response = tracked ? response : response.AsNoTracking();
            response = filter == null ? response : response.Where(filter);

            return await response.ToListAsync();
        }

        /************************************************************************
         *                       Check The User Exist
         ***********************************************************************/
        public async Task<User> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
