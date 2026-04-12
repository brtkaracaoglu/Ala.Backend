using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using System.Security.Cryptography;
using System.Text;

namespace Ala.Backend.Infrastructure.Services.Token
{
    public class RefreshTokenHasher : IRefreshTokenHasher
    {
        public string Hash(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }
    }
}