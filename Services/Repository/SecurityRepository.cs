using DMNRestaurant.Areas.Identity.Data;
using DMNRestaurant.Helper.Enum;
using DMNRestaurant.Services.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DMNRestaurant.Services.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly IConfiguration _config;

        public SecurityRepository(IConfiguration config)
        {
            _config = config;
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public string GenerateJwtToken(User user, IList<string> roles)
        {
            var claimes = new List<Claim> {
                new Claim(EJwtAliasKey.user_id.ToString(), user.Id),
                new Claim(EJwtAliasKey.email.ToString(), user.Email),
                new Claim(EJwtAliasKey.roles.ToString(), String.Join(",", roles))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _config.GetSection("SecuritySettings:JwtSecretKey").Value)
                );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claimes),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
