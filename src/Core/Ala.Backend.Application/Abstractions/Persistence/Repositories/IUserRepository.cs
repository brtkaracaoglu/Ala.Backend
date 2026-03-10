using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories
{
    public interface IUserRepository
    {
        Task<IList<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    }
}