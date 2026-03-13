using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.UserRoles.AddRoleToUser
{
    public class AddRoleToUserCommandValidator : AbstractValidator<AddRoleToUserCommandRequest>
    {
        public AddRoleToUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Kullanıcı ID geçerli (sıfırdan büyük) olmalıdır.");

            RuleFor(x => x.RoleId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol ID zorunludur.")
                .GreaterThan(0).WithMessage("Rol ID geçerli (sıfırdan büyük) olmalıdır.");
        }
    }
}