using FluentValidation;
using System.Text.RegularExpressions;

namespace Ala.Backend.Application.Features.Commands.Auth.ForgotPassword
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommandRequest>
    {
        private const int MaxEmailLength = 256;

        private static readonly Regex EmailRegex =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ForgotPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("E-posta adresi zorunludur.")
                .MaximumLength(MaxEmailLength)
                    .WithMessage("E-posta adresi çok uzun.")
                .Must(IsValidEmailFormat)
                    .WithMessage("Geçerli bir e-posta adresi giriniz.");
        }

        private static bool IsValidEmailFormat(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return EmailRegex.IsMatch(email);
        }
    }
}
