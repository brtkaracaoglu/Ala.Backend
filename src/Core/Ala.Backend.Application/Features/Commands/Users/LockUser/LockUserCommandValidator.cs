using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Users.LockUser
{
    public class LockUserCommandValidator : AbstractValidator<LockUserCommandRequest>
    {
        public LockUserCommandValidator()
        {
            RuleFor(x => x.Id)
               .Cascade(CascadeMode.Stop)
               .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
               .GreaterThan(0).WithMessage("Kullanıcı ID geçerli (sıfırdan büyük) bir değer olmalıdır.");

            RuleFor(x => x.DurationInHours)
                .GreaterThan(0)
                .When(x => x.DurationInHours.HasValue)
                .WithMessage("Belirtilen kilit süresi (saat cinsinden) 0'dan büyük olmalıdır.");
        }
    }
}