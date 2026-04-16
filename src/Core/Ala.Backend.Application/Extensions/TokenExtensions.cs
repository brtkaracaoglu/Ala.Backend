using System.Text;

namespace Ala.Backend.Application.Extensions
{
    public static class TokenExtensions
    {
        public static string EncodeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token boş olamaz.", nameof(token));

            var bytes = Encoding.UTF8.GetBytes(token);
            var base64 = Convert.ToBase64String(bytes);

            return base64
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static string DecodeToken(string urlEncodedToken)
        {
            if (string.IsNullOrWhiteSpace(urlEncodedToken))
                throw new ArgumentException("Encoded token boş olamaz.", nameof(urlEncodedToken));

            var base64 = urlEncodedToken
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
                case 0:
                    break;
                default:
                    throw new FormatException("Geçersiz Base64Url token formatı.");
            }

            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}