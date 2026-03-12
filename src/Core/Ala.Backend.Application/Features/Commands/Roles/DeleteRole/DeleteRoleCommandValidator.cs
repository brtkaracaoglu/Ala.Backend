using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Roles.DeleteRole
{
    public class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommandRequest>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol ID zorunludur.")
                .GreaterThan(0).WithMessage("Geçerli bir rol ID (sıfırdan büyük) girilmelidir.");
        }
    }
}