using DMNRestaurant.Areas.Identity.Data;

namespace DMNRestaurant.Services.Repository.IRepository
{
    public interface ISecurityRepository
    {
        string GenerateJwtToken(User user, IList<string> roles);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
