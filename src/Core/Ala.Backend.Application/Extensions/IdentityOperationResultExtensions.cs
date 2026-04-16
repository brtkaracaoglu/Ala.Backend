using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Identity;

namespace Ala.Backend.Application.Extensions
{
    public static class IdentityOperationResultExtensions
    {
        public static void ThrowIfFailed(this IdentityOperationResult result, string detail)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.Succeeded)
                return;

            var errors = result.Errors
                .GroupBy(x => string.IsNullOrWhiteSpace(x.PropertyName) ? "General" : x.PropertyName!)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Description)
                          .Where(x => !string.IsNullOrWhiteSpace(x))
                          .Distinct()
                          .ToArray()
                );

            throw new AppValidationException(detail, errors);
        }
    }
}