using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToUser
{
    public class AssignPermissionToUserCommandValidator : AbstractValidator<AssignPermissionToUserCommandRequest>
    {
        private const int MaxPermissionCodeLength = 100;

        public AssignPermissionToUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Kullanıcı ID geçerli (sıfırdan büyük) olmalıdır.");

            RuleFor(x => x.PermissionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("İzin Kodu (Permission Code) zorunludur.")
                .MaximumLength(MaxPermissionCodeLength).WithMessage($"İzin Kodu en fazla {MaxPermissionCodeLength} karakter olabilir.");
        }
    }
}
