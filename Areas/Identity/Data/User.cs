using Microsoft.AspNetCore.Identity;

namespace DMNRestaurant.Areas.Identity.Data;

// Add profile data for application users by adding properties to the User class
public class User : IdentityUser
{
    public string FullName { get; set; }
    public string Photo { get; set; }
    public string Address { get; set; }
}

