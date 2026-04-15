using Ala.Backend.Application.Common.Requests;

namespace Ala.Backend.Application.DTOs.UserRoles
{
    public class UserRoleListFilter : PagedFilterRequest
    {
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}

