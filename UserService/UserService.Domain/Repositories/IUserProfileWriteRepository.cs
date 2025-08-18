using DotnetBaseKit.Components.Domain.Sql.Repositories;
using UserService.Domain.Entities;

namespace UserService.Domain.Repositories;

public interface IUserProfileWriteRepository : IBaseWriteRepository<UserProfile>
{
    
}