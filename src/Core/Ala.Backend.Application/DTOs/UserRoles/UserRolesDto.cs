namespace Ala.Backend.Application.DTOs.UserRoles
{
    public class UserRolesDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
    }
}
