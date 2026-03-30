using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Infra.Sql.Repositories.Base;

namespace AuthService.Infra.Repositories;

public class ResetPasswordTokenWriteRepository : BaseWriteRepository<ResetPasswordToken>, IResetPasswordTokenWriteRepository
{
    public ResetPasswordTokenWriteRepository(BaseContext context) : base(context)
    {

    }
}