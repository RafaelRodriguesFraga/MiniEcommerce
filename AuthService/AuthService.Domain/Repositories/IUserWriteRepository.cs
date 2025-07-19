using AuthService.Domain.Entities;
using DotnetBaseKit.Components.Domain.Sql.Repositories;

namespace AuthService.Domain.Repositories;

public interface IUserWriteRepository : IBaseWriteRepository<User>
{
    Task<bool> GetByEmailAsync(string email);
}