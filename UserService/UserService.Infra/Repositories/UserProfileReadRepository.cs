using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Infra.Repositories;

public class UserProfileReadRepository : BaseReadRepository<UserProfile>, IUserProfileReadRepository
{
    public UserProfileReadRepository(BaseContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(Guid userId)
    {
        return await Set.Where(p => p.UserId == userId).FirstOrDefaultAsync();
    }
}