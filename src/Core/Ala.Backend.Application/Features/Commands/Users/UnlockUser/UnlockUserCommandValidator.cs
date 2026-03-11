using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Users.UnlockUser
{
    public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommandRequest>
    {
        public UnlockUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı kimliği (Id) girmelisiniz.");
        }
    }
}