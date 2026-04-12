using FluentValidation;
using System.Text.RegularExpressions;

namespace Ala.Backend.Application.Features.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommandRequest>
    {
        private const int MinPasswordLength = 6;
        private const int MaxPasswordLength = 20;

        //  Kurumsal password policy regex
        // En az 1 büyük, 1 küçük, 1 rakam, 1 özel karakter
        private static readonly Regex StrongPasswordRegex =
            new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
                RegexOptions.Compiled);

        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Mevcut şifre zorunludur.");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Yeni şifre zorunludur.")
                .MinimumLength(MinPasswordLength)
                    .WithMessage($"Yeni şifre en az {MinPasswordLength} karakter olmalıdır.")
                .MaximumLength(MaxPasswordLength)
                    .WithMessage($"Yeni şifre en fazla {MaxPasswordLength} karakter olabilir.")
                .Must(IsStrongPassword)
                    .WithMessage(
                        "Yeni şifre; büyük harf, küçük harf, rakam ve özel karakter içermelidir.")
                .Must((req, newPassword) => newPassword != req.OldPassword)
                    .WithMessage("Yeni şifre, mevcut şifre ile aynı olamaz.");

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage("Yeni şifre onayı zorunludur.")
                .Equal(x => x.NewPassword)
                .WithMessage("Yeni şifre ile onay şifresi eşleşmiyor.");
        }

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return StrongPasswordRegex.IsMatch(password);
        }
    }
}
