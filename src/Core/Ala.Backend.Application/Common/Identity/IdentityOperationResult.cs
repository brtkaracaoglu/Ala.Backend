namespace Ala.Backend.Application.Common.Identity
{
    public class IdentityOperationResult
    {
        private static readonly IdentityOperationResult _success = new()
        {
            Succeeded = true
        };

        public bool Succeeded { get; init; }

        public IReadOnlyCollection<IdentityOperationError> Errors { get; init; }
            = Array.Empty<IdentityOperationError>();

        public static IdentityOperationResult Success() => _success;

        public static IdentityOperationResult Failed(IEnumerable<IdentityOperationError> errors)
        {
            return new IdentityOperationResult
            {
                Succeeded = false,
                Errors = errors?.ToArray() ?? Array.Empty<IdentityOperationError>()
            };
        }

        public static IdentityOperationResult Failed(params IdentityOperationError[] errors)
        {
            return new IdentityOperationResult
            {
                Succeeded = false,
                Errors = errors?.Length > 0
                    ? errors
                    : Array.Empty<IdentityOperationError>()
            };
        }
    }
}