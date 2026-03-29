using DotnetBaseKit.Components.Application.Base;

namespace AuthService.Application.Services.ResetPassword;

public interface IResetPasswordServiceApplication : IBaseServiceApplication
{
    Task ResetPasswordAsync(string token, string newPassword);
}