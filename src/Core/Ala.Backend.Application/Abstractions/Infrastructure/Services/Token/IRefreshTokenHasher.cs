namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Token
{
    public interface IRefreshTokenHasher
    {
        string Hash(string token);
    }
}