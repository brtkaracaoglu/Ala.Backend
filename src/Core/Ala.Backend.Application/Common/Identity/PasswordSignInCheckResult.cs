namespace Ala.Backend.Application.Common.Identity
{
    public class PasswordSignInCheckResult
    {
        private static readonly PasswordSignInCheckResult _success = new()
        {
            Succeeded = true
        };

        public bool Succeeded { get; init; }
        public bool IsLockedOut { get; init; }
        public bool IsNotAllowed { get; init; }
        public bool RequiresTwoFactor { get; init; }

        public static PasswordSignInCheckResult Success() => _success;

        public static PasswordSignInCheckResult Failed(
            bool isLockedOut = false,
            bool isNotAllowed = false,
            bool requiresTwoFactor = false)
        {
            return new PasswordSignInCheckResult
            {
                Succeeded = false,
                IsLockedOut = isLockedOut,
                IsNotAllowed = isNotAllowed,
                RequiresTwoFactor = requiresTwoFactor
            };
        }
    }
}