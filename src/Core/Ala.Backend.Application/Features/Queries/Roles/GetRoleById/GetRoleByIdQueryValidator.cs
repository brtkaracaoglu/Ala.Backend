using FluentValidation;

namespace Ala.Backend.Application.Features.Queries.Roles.GetRoleById
{
    public class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQueryRequest>
    {
        public GetRoleByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID (sıfırdan büyük) giriniz.");
        }
    }
}
