using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Users.SetActiveUser
{
    public class SetActiveUserCommandValidator : AbstractValidator<SetActiveUserCommandRequest>
    {
        public SetActiveUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Kullanıcı ID geçerli (sıfırdan büyük) bir değer olmalıdır.");
        }
    }
}