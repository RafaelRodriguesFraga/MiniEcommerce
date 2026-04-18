using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Shared;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services.Auth;

public class AuthServiceApplication : BaseServiceApplication, IAuthServiceApplication
{
    private readonly IUserReadRepository _readRepository;
    private readonly IUserWriteRepository _writeRepository;
    private readonly IResetPasswordTokenWriteRepository _tokenWriteRepository;
    private readonly IResetPasswordTokenReadRepository _tokenReadRepository;

    public AuthServiceApplication(
        NotificationContext notificationContext,
        IUserWriteRepository writeRepository,
        IUserReadRepository readRepository,
        IResetPasswordTokenWriteRepository tokenWriteRepository,
        IResetPasswordTokenReadRepository tokenReadRepository) : base(notificationContext)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _tokenWriteRepository = tokenWriteRepository;
        _tokenReadRepository = tokenReadRepository;
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _readRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        var rawToken = Guid.NewGuid().ToString();

        var resetToken = new ResetPasswordToken(user.Id, rawToken);

        await _tokenWriteRepository.InsertAsync(resetToken);

        var link = $"http://localhost:3000/reset-password?token={rawToken}&email={email}";
        Console.WriteLine($"\n=============================================");
        Console.WriteLine($"📧 MOCK DE EMAIL: Link gerado para {email}");
        Console.WriteLine($"🔗 CLIQUE AQUI: {link}");
        Console.WriteLine($"=============================================\n");

    }

    public async Task ResetPasswordAsync(string email, string newPassword, string rawToken)
    {

        var incomingHash = TokenHasher.HashToken(rawToken);

        var tokenEntity = await _tokenReadRepository.GetByTokenHashAsync(incomingHash);

        if (tokenEntity == null || tokenEntity.Used || tokenEntity.ExpirationDate < DateTime.UtcNow)
        {
            _notificationContext.AddNotification("Token", "Token inválido, expirado ou já utilizado.");
            return;
        }

        var user = await _readRepository.GetByEmailAsync(email);

        if (user == null || user.Id != tokenEntity.UserId)
        {
            _notificationContext.AddNotification("User", "Usuário não encontrado ou token não pertence a este e-mail.");
            return;
        }

        user!.SetPassword(newPassword);

        await _writeRepository.UpdateAsync(user);
    }
}