using FluentValidation;

namespace Ala.Backend.Application.Features.Commands.Users.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommandRequest>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter olmalıdır.")
                .MaximumLength(256).WithMessage("Kullanıcı adı en fazla 256 karakter olabilir.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
                .EmailAddress().WithMessage("Lütfen geçerli bir e-posta adresi giriniz.")
                .MaximumLength(256).WithMessage("E-posta adresi en fazla 256 karakter olabilir.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Ad alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .MaximumLength(100).WithMessage("Soyad alanı en fazla 100 karakter olabilir.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifre boş bırakılamaz.")
                .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır.")
                .Matches("[A-Z]").WithMessage("Şifreniz en az bir büyük harf içermelidir.")
                .Matches("[a-z]").WithMessage("Şifreniz en az bir küçük harf içermelidir.")
                .Matches("[0-9]").WithMessage("Şifreniz en az bir rakam içermelidir.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Şifreniz en az bir özel karakter (örn: !@#$%^&*) içermelidir.");
        }
    }
}