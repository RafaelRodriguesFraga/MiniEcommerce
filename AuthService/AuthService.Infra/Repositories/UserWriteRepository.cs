using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infra.Context;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra.Repositories;

public class UserWriteRepository : BaseWriteRepository<User>, IUserWriteRepository
{
   
    public UserWriteRepository(BaseContext context) : base(context)
    {
       
    }

    public async Task<bool> GetByEmailAsync(string email)
    {
        return await Set.AnyAsync(u => u.Email == email);
    }
}