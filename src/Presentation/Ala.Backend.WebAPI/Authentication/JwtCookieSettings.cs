using Microsoft.AspNetCore.Http;

namespace Ala.Backend.WebAPI.Authentication
{
    public sealed class JwtCookieSettings
    {
        public const string SectionName = "JwtCookieSettings";

        public bool HttpOnly { get; set; } = true;
        public bool Secure { get; set; } = true;
        public SameSiteMode SameSite { get; set; } = SameSiteMode.None;
        public bool IsEssential { get; set; } = true;

        public string AccessTokenPath { get; set; } = "/";
        public string RefreshTokenPath { get; set; } = "/";

    }
}