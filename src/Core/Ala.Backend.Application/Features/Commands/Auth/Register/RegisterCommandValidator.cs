using FluentValidation;
using System.Text.RegularExpressions;

namespace Ala.Backend.Application.Features.Commands.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommandRequest>
    {
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 100;

        private const int MinUserNameLength = 3;
        private const int MaxUserNameLength = 50;

        private const int MinNameLength = 2;
        private const int MaxNameLength = 100;

        private const int MaxEmailLength = 256;

        private const int MinPhoneLength = 10;
        private const int MaxPhoneLength = 20;

        private static readonly Regex StrongPasswordRegex =
            new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$", RegexOptions.Compiled);

        private static readonly Regex NameRegex =
            new(@"^[a-zA-ZğĞüÜşŞıİöÖçÇ\s]+$", RegexOptions.Compiled);

        private static readonly Regex PhoneRegex =
            new(@"^[\d\+\-\(\)\s]+$", RegexOptions.Compiled);

        public RegisterCommandValidator()
        {
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                .MinimumLength(MinUserNameLength).WithMessage($"Kullanıcı adı en az {MinUserNameLength} karakter olmalıdır.")
                .MaximumLength(MaxUserNameLength).WithMessage($"Kullanıcı adı en fazla {MaxUserNameLength} karakter olabilir.")
                .Matches(@"^[a-zA-Z0-9_.]+$").WithMessage("Kullanıcı adı yalnızca harf, rakam, nokta ve alt çizgi içerebilir.");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
                .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.")
                .MaximumLength(MaxEmailLength).WithMessage($"E-posta adresi en fazla {MaxEmailLength} karakter olabilir.");

            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .MinimumLength(MinNameLength).WithMessage($"Ad alanı en az {MinNameLength} karakter olmalıdır.")
                .MaximumLength(MaxNameLength).WithMessage($"Ad alanı en fazla {MaxNameLength} karakter olabilir.")
                .Matches(NameRegex).WithMessage("Ad yalnızca harf ve boşluk içerebilir.");

            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .MinimumLength(MinNameLength).WithMessage($"Soyad alanı en az {MinNameLength} karakter olmalıdır.")
                .MaximumLength(MaxNameLength).WithMessage($"Soyad alanı en fazla {MaxNameLength} karakter olabilir.")
                .Matches(NameRegex).WithMessage("Soyad yalnızca harf ve boşluk içerebilir.");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                .MinimumLength(MinPasswordLength).WithMessage($"Şifre en az {MinPasswordLength} karakter olmalıdır.")
                .MaximumLength(MaxPasswordLength).WithMessage($"Şifre en fazla {MaxPasswordLength} karakter olabilir.")
                .Must(IsStrongPassword).WithMessage("Şifre; en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.");

            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .MinimumLength(MinPhoneLength).WithMessage($"Telefon numarası en az {MinPhoneLength} karakter olmalıdır.")
                .MaximumLength(MaxPhoneLength).WithMessage($"Telefon numarası en fazla {MaxPhoneLength} karakter olabilir.")
                .Matches(PhoneRegex).WithMessage("Telefon numarası yalnızca rakam ve geçerli karakterler (+, -, boşluk, parantez) içerebilir.")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }

        private static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return StrongPasswordRegex.IsMatch(password);
        }
    }
}