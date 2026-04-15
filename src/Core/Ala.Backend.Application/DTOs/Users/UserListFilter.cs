using Ala.Backend.Application.Common.Requests;

namespace Ala.Backend.Application.DTOs.Users
{
    public class UserListFilter : PagedFilterRequest
    {
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
    }
}
