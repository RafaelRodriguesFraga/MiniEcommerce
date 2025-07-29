using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services.Auth;

public class AuthServiceApplication : BaseServiceApplication, IAuthServiceApplication
{
    private readonly IUserReadRepository _readRepository;
    private readonly IUserWriteRepository _writeRepository;
    
    public AuthServiceApplication(NotificationContext notificationContext, IUserWriteRepository writeRepository, IUserReadRepository readRepository) : base(notificationContext)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _readRepository.GetByEmailAsync(email);
        var userNotFound = user == null;
        if (userNotFound)
        {
            _notificationContext.AddNotification("User", "User not found");
            return;
        }

        user!.SetPassword(newPassword);

        await _writeRepository.UpdateAsync(user);
    }
}