using Microsoft.AspNetCore.Identity;

namespace Ala.Backend.Domain.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } =null!;
        public string LastName { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
