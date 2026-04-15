using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;

namespace Ala.Backend.Application.Abstractions.Persistence.Service.Queries
{
    public interface IUserQueryService
    {
        Task<PagedResponse<UserDto>> GetPagedAsync(UserListFilter request, CancellationToken cancellationToken = default);
    }
}
