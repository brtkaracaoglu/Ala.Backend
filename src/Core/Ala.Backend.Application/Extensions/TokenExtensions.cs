using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Ala.Backend.Application.Extensions
{
    public static class TokenExtensions
    {
        public static string EncodeToken(string token)
        {
            byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(token);
            return WebEncoders.Base64UrlEncode(tokenGeneratedBytes);
        }

        public static string DecodeToken(string urlEncodedToken)
        {
            byte[] tokenDecodedBytes = WebEncoders.Base64UrlDecode(urlEncodedToken);
            return Encoding.UTF8.GetString(tokenDecodedBytes);
        }
    }
}
