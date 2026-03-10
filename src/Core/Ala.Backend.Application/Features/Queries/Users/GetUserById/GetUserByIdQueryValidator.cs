using FluentValidation;

namespace Ala.Backend.Application.Features.Queries.Users.GetUserById
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQueryRequest>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Kullanıcı ID zorunludur.")
                .GreaterThan(0).WithMessage("Geçerli bir kullanıcı ID (sıfırdan büyük) giriniz.");
        }
    }
}
