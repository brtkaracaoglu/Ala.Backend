using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommandRequest>
    {
        private const int MaxPermissionCodeLength = 100;

        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol ID zorunludur.")
                .GreaterThan(0).WithMessage("Rol ID geçerli (sıfırdan büyük) olmalıdır.");

            RuleFor(x => x.PermissionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("İzin Kodu (Permission Code) zorunludur.")
                .MaximumLength(MaxPermissionCodeLength).WithMessage($"İzin Kodu en fazla {MaxPermissionCodeLength} karakter olabilir.");
        }
    }
}
