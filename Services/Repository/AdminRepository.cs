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
    public class AdminRepository : IAdminRepository
    {
        private readonly ILogger<AdminRepository> logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPhotoRepository _photoRepository;

        public AdminRepository(
                ILogger<AdminRepository> logger,
                IMapper mapper,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                RoleManager<IdentityRole> roleManager,
                IPhotoRepository photoRepository
            )
        {
            this.logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _photoRepository = photoRepository;
        }

        /************************************************************************
         *                        Create New an Account
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> CreateAsync(User user)
        {
            int statusCode = 201;
            var errMessage = new List<string>();

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                statusCode = 400;
                errMessage.AddRange(GetErrorMessages(result));
                //foreach (var item in result.Errors)
                //{
                //    errMessage.Add(item.Description);
                //}
            }

            if (!await _roleManager.RoleExistsAsync(ERoles.Member.ToString()))
            {
                var role = new IdentityRole(ERoles.Member.ToString());
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, ERoles.Member.ToString());
            return (statusCode, errMessage);
        }

        /************************************************************************
         *                        Get a Single Account
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage, UserRolesDTO userRolesDTO)>
            GetSingleUserAsync(string userId)
        {
            int statusCode = 200;
            var errMessage = new List<string>();
            var userRoles = new UserRolesDTO();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                statusCode = 404;
                errMessage.Add("Account not found");
                return (statusCode, errMessage, userRoles);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null)
            {
                statusCode = 404;
                errMessage.Add("Roles account not found");
            }
            else
            {
                userRoles = GetSingleUserRoles(user, roles);
            }

            return (statusCode, errMessage, userRoles);
        }

        /************************************************************************
         *                   Get Manay Accounts With Filter
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage, List<UsersWithPaginationDTO> userDTOs)>
            GetUsersAsync(
                string scheme,
                string host,
                Expression<Func<User, bool>>? filter,
                int page,
                int pageSize,
                bool tracked = true)
        {
            int statusCode = 200;
            var errMessage = new List<string>();
            var userList = new List<UsersWithPaginationDTO>();
            long total = 0;

            IQueryable<User>? query = _userManager.Users;

            if (query == null)
            {
                statusCode = 404;
                errMessage.Add("Account not found.");

                return (statusCode, errMessage, userList);
            }
            else
            {
                query = tracked ? query : query.AsNoTracking();
                query = filter == null ? query : query.Where(filter);

                total = await query.CountAsync();
                var users = await query
                                    .OrderBy(o => o.Email)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

                var usersDTO = new List<UserDTO>();
                foreach (var item in users)
                {
                    item.Photo = item.Photo == "nopic.png"
                        ? item.Photo
                        : $"{scheme}://{host}/images/accounts/{item.Photo}";

                    usersDTO.Add(_mapper.Map<UserDTO>(item));
                }

                userList.Add(new UsersWithPaginationDTO
                {
                    Users = usersDTO,
                    Total = total,
                    Page = page,
                    PageSize = pageSize
                });
            }

            return (statusCode, errMessage, userList);
        }

        /************************************************************************
         *                        Update an Account
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> UpdateAsync(string userId, UserUpdateDTO dto, IFormFile file)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                statusCode = 404;
                errMessage.Add("Account not found");
                return (statusCode, errMessage);
            }

            // If user attached file for upload
            if (_photoRepository.IsUpdaload(file))
            {
                var validResult = _photoRepository.Validation(file);
                if (!String.IsNullOrEmpty(validResult))
                {
                    statusCode = 400;
                    errMessage.Add(validResult);
                    return (statusCode, errMessage);
                }

                if (user.Photo != "nopic.png")
                    _photoRepository.Remove(user.Photo, "accounts");

                var uploadResult = await _photoRepository.UploadAsync(file, "accounts");
                user.Photo = uploadResult;
            }

            // Update new account
            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                statusCode = 400;
                foreach (var item in result.Errors)
                {
                    errMessage.Add(item.Description);
                }
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                        Delete an Account
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> DeleteAsync(User user)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                statusCode = 400;
                errMessage.AddRange(GetErrorMessages(result));
            }

            if (user.Photo != "nopic.png")
                _photoRepository.Remove(user.Photo, "accounts");

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                           Update Password
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> UpdatePasswordAsync(string userId, AdminUpdatePasswordDTO dto)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                statusCode = 404;
                errMessage.Add("Account not found");
                return (statusCode, errMessage);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, dto.Password);

            if (!result.Succeeded)
            {
                statusCode = 400;
                errMessage.Add("Invalid update password. Please try again");
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                     Update New Roles To Account
         ***********************************************************************/
        public async Task<(int statusCode, List<string> errMessage)> UpdateRolesAsync(string userId, AdminUpdateRolesDTO dto)
        {
            int statusCode = 200;
            var errMessage = new List<string>();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                statusCode = 404;
                errMessage.Add("Account not found");
                return (statusCode, errMessage);
            }

            var rolesToRemove = await _userManager.GetRolesAsync(user);
            // Remove old roles from db
            foreach (var item in rolesToRemove)
            {
                await _userManager.RemoveFromRoleAsync(user, item);
            }

            string[] rolesToUpdate = dto.Roles.Split(","); // Member,Guest,Admin
            // Add new role to db
            foreach (var item in rolesToUpdate)
            {
                var result = await _userManager.AddToRoleAsync(user, item);
                if (result.Succeeded)
                {
                    statusCode = 400;
                    errMessage.Add("Invalid edit role");
                    return (statusCode, errMessage);
                }
            }

            return (statusCode, errMessage);
        }

        /************************************************************************
         *                        Check Exist Account
         ***********************************************************************/
        public async Task<User> UserExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /************************************************************************
         *                   Handle Errors - Helper Method
         ***********************************************************************/
        public List<string> GetErrorMessages(IdentityResult result)
        {
            var errMessage = new List<string>();

            foreach (var err in result.Errors)
            {
                errMessage.Add(err.Description);
            }

            return errMessage;
        }

        /************************************************************************
         *                   Get a Single User & Roles
         ***********************************************************************/
        public UserRolesDTO GetSingleUserRoles(User user, IList<string> roles)
        {
            return new UserRolesDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Photo = user.Photo,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Roles = roles
            };
        }

    }
}
