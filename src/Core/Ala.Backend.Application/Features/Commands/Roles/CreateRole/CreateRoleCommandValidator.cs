using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Roles.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommandRequest>
    {
        private const int MinRoleNameLength = 3;
        private const int MaxRoleNameLength = 50;

        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Rol adı zorunludur.")
                .MinimumLength(MinRoleNameLength)
                    .WithMessage($"Rol adı en az {MinRoleNameLength} karakter olmalıdır.")
                .MaximumLength(MaxRoleNameLength)
                    .WithMessage($"Rol adı en fazla {MaxRoleNameLength} karakter olabilir.");
        }
    }
}
