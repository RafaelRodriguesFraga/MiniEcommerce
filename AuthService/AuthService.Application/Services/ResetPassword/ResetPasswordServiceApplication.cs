using AuthService.Domain.Repositories;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services.ResetPassword;

public class ResetPasswordServiceApplication : BaseServiceApplication, IResetPasswordServiceApplication
{

    private readonly IResetPasswordTokenReadRepository _tokenReadRepository;
    private readonly IResetPasswordTokenWriteRepository _tokenWriteRepository;
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUserWriteRepository _userWriteRepository;

    public ResetPasswordServiceApplication(
        NotificationContext notificationContext,
        IResetPasswordTokenReadRepository tokenReadRepository,
        IResetPasswordTokenWriteRepository tokenWriteRepository,
        IUserReadRepository userReadRepository,
        IUserWriteRepository userWriteRepository) : base(notificationContext)
    {
        _tokenReadRepository = tokenReadRepository;
        _tokenWriteRepository = tokenWriteRepository;
        _userReadRepository = userReadRepository;
        _userWriteRepository = userWriteRepository;
    }

    public async Task ResetPasswordAsync(string token, string newPassword)
    {
        var tokenHash = TokenHasher.HashToken(token);

        var tokenEntity = await _tokenReadRepository.GetByTokenHashAsync(tokenHash);

        if (tokenEntity == null || tokenEntity.Used || tokenEntity.ExpirationDate < DateTime.UtcNow)
        {
            _notificationContext.AddNotification("Token", "Token inválido ou expirado.");
            return;
        }

        var user = await _userReadRepository.GetByIdAsync(tokenEntity.UserId);
        if (user == null || user.Id != tokenEntity.UserId)
        {
            _notificationContext.AddNotification("User", "Usuário não encontrado.");
            return;
        }

        user.SetPassword(newPassword);
        await _userWriteRepository.UpdateAsync(user);

        tokenEntity.MarkAsUsed();
        await _tokenWriteRepository.UpdateAsync(tokenEntity);
    }
}