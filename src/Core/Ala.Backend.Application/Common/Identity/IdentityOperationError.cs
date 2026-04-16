namespace Ala.Backend.Application.Common.Identity
{
    public class IdentityOperationError
    {
        public string Code { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? PropertyName { get; init; }
    }
}