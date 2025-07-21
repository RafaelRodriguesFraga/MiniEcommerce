using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infra.Context;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra.Repositories;

public class UserReadRepository : BaseReadRepository<User>, IUserReadRepository
{
   
    public UserReadRepository(BaseContext context) : base(context)
    {
       
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Set.FirstOrDefaultAsync(u => u.Email == email);
    }
}