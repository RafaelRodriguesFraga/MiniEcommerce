using AuthService.Domain.Entities;
using DotnetBaseKit.Components.Domain.Sql.Repositories;

namespace AuthService.Domain.Repositories;

public interface IUserReadRepository : IBaseReadRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}