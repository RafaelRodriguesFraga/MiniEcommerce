using DotnetBaseKit.Components.Domain.Sql.Repositories;
using UserService.Domain.Entities;

namespace UserService.Domain.Repositories;

public interface IUserProfileReadRepository : IBaseReadRepository<UserProfile>
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId);
}