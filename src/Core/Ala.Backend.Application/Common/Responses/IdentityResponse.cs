namespace Ala.Backend.Application.Common.Responses
{
    public record IdentityResponse
    {
        public bool Succeeded { get; init; }
        public IEnumerable<string> Errors { get; init; }

        // SignIn'e özel ek durumlar
        public bool IsLockedOut { get; init; }
        public bool IsNotAllowed { get; init; }
        public bool RequiresTwoFactor { get; init; }

        // Normal Identity İşlemleri İçin Başarı (Create, Update, vb.)
        public static IdentityResponse Success() => new()
        {
            Succeeded = true,
            Errors = Enumerable.Empty<string>()
        };

        // Giriş İşlemi İçin Başarı
        public static IdentityResponse SignInSuccess() => new()
        {
            Succeeded = true,
            Errors = Enumerable.Empty<string>()
        };

        // Normal Hata Durumu (IdentityResult)
        public static IdentityResponse Failure(IEnumerable<string> errors) => new()
        {
            Succeeded = false,
            Errors = errors
        };

        // SignIn Hata Durumu (SignInResult)
        public static IdentityResponse SignInFailure(bool isLockedOut = false, bool isNotAllowed = false, bool requiresTwoFactor = false) => new()
        {
            Succeeded = false,
            IsLockedOut = isLockedOut,
            IsNotAllowed = isNotAllowed,
            RequiresTwoFactor = requiresTwoFactor,
            Errors = new[] { "Giriş işlemi başarısız." } // Varsayılan bir hata mesajı
        };
    }
}