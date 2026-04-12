//using FluentValidation;
//using System.Text.RegularExpressions;

//namespace Ala.Backend.Application.Features.Commands.Auth.RefreshToken
//{
//    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommandRequest>
//    {
//        // URL-safe token (Base64 / Identity token uyumlu) format kontrolü
//        private static readonly Regex RefreshTokenFormatRegex =
//            new(@"^[A-Za-z0-9\-_+=/%]+$",
//                RegexOptions.Compiled);

//        public RefreshTokenCommandValidator()
//        {
//            RuleFor(x => x.RefreshToken)
//                 .Cascade(CascadeMode.Stop)
//                .NotEmpty().WithMessage("Refresh token'ı zorunludur.")
//                .Matches(RefreshTokenFormatRegex).WithMessage("Geçersiz refresh token formatı.");

//        }
//    }
//}
