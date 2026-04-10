using Ala.Backend.Application.Common.Models;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions
{
    public interface IClientInfoParser
    {
        ParsedClientInfo Parse(string? userAgent);
    }
}