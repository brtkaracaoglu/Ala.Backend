using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IUserRepository
    {
        Task<PagedResponse<UserDto>> GetPagedUsersWithRolesAsync(UserListFilter filter, CancellationToken cancellationToken = default);
    }
}
