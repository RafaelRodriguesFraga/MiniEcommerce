using AuthService.Domain.Entities;
using DotnetBaseKit.Components.Domain.Sql.Repositories;

namespace AuthService.Domain.Repositories;

public interface IResetPasswordTokenReadRepository : IBaseReadRepository<ResetPasswordToken>
{
    Task<ResetPasswordToken?> GetByTokenHashAsync(string tokenHash);
}