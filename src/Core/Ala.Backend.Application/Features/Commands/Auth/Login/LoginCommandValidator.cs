using FluentValidation;
using System.Text.RegularExpressions;

namespace Ala.Backend.Application.Features.Commands.Auth.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommandRequest>
    {
        private const int MinUsernameLength = 3;
        private const int MaxUsernameLength = 25;
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 20;

        private static readonly Regex UserNameRegex =
            new(@"^[a-zA-Z0-9._@-]+$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public LoginCommandValidator()
        {
            RuleFor(x => x.EmailOrUsername)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("E-posta veya Kullanıcı adı zorunludur.")
                .MinimumLength(MinUsernameLength)
                    .WithMessage($"En az {MinUsernameLength} karakter olmalıdır.")
                .MaximumLength(MaxUsernameLength)
                    .WithMessage($"En fazla {MaxUsernameLength} karakter olabilir.")
                .Must(BeValidUserNameOrEmail)
                    .WithMessage("Geçersiz kullanıcı adı veya e-posta formatı.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Şifre zorunludur.")
                .MinimumLength(MinPasswordLength)
                    .WithMessage($"Şifre en az {MinPasswordLength} karakter olmalıdır.")
                .MaximumLength(MaxPasswordLength)
                    .WithMessage($"Şifre en fazla {MaxPasswordLength} karakter olabilir.");
        }

        private static bool BeValidUserNameOrEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            if (value.Contains("@"))
                return value.Length <= MaxUsernameLength;

            return UserNameRegex.IsMatch(value);
        }
    }
}
