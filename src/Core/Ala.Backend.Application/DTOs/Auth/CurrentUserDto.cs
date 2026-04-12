namespace Ala.Backend.Application.DTOs.Auth
{
    public sealed class CurrentUserDto
    {
        public int? Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
        public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();

        public bool IsAuthenticated { get; set; }
    }
}