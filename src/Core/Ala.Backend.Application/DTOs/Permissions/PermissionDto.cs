namespace Ala.Backend.Application.DTOs.Permissions
{
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
    }
}
