using DotnetBaseKit.Components.Domain.Sql.Repositories;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Infra.Repositories;

public class UserProfileWriteRepository : BaseWriteRepository<UserProfile>, IUserProfileWriteRepository
{
    public UserProfileWriteRepository(BaseContext context) : base(context)
    {
    }
}