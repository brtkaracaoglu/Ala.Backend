using FluentValidation;
using System.Text.RegularExpressions;

namespace Ala.Backend.Application.Features.Commands.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommandRequest>
    {
        // URL-safe token (Base64 / Identity token uyumlu) format kontrolü
        private static readonly Regex TokenFormatRegex =
            new(@"^[A-Za-z0-9\-_+=/%]+$",
                RegexOptions.Compiled);

        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID giriniz.");

            RuleFor(x => x.Token)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Doğrulama token'ı zorunludur.")
                .Matches(TokenFormatRegex).WithMessage("Geçersiz doğrulama token formatı.");
        }
    }
}
