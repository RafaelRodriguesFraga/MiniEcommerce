using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services.Auth;

public class AuthServiceApplication : BaseServiceApplication, IAuthServiceApplication
{
    private readonly IUserReadRepository _readRepository;
    private readonly IUserWriteRepository _writeRepository;
    private readonly IResetPasswordTokenWriteRepository _tokenWriteRepository;

    public AuthServiceApplication(
        NotificationContext notificationContext,
        IUserWriteRepository writeRepository,
        IUserReadRepository readRepository,
        IResetPasswordTokenWriteRepository tokenWriteRepository) : base(notificationContext)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _tokenWriteRepository = tokenWriteRepository;
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _readRepository.GetByEmailAsync(email);
        if (user == null)
        {
            _notificationContext.AddNotification("User", "Usuário não encontrado");
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