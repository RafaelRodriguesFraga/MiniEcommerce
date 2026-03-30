using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Shared;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra.Repositories;

public class ResetPasswordTokenReadRepository : BaseReadRepository<ResetPasswordToken>, IResetPasswordTokenReadRepository
{
    public ResetPasswordTokenReadRepository(BaseContext context) : base(context)
    {

    }

    public async Task<ResetPasswordToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await Set.AsNoTracking().SingleOrDefaultAsync(u => u.TokenHash == tokenHash);
    }
}