//using FluentValidation;

//namespace Ala.Backend.Application.Features.Commands.Auth.Logout
//{
//    public class LogoutCommandValidator : AbstractValidator<LogoutCommandRequest>
//    {
//        public LogoutCommandValidator() 
//        {
//            RuleFor(x => x.UserId)
//                .Cascade(CascadeMode.Stop)
//                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
//                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID giriniz.");
//        }
//    }
//}
