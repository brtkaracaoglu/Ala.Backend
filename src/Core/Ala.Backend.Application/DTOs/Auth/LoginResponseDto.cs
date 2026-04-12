
namespace Ala.Backend.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string FullName { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public bool RequiresPasswordReset { get; set; }
        public string? ResetPasswordToken { get; set; }
    }
}
