using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs.User;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services.User;

public class UserService : BaseServiceApplication, IUserService
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUserWriteRepository _writeRepository;

    public UserService(NotificationContext notificationContext, IUserReadRepository userReadRepository,
        IUserWriteRepository writeRepository)
        : base(notificationContext)
    {
        _userReadRepository = userReadRepository;
        _writeRepository = writeRepository;
    }

    public async Task<UserResponseDto> AuthenticateAsync(LoginRequestDto requestDto)
    {
        var user = await _userReadRepository.GetByEmailAsync(requestDto.Email);
        var userNotFound = user == null;
        if (userNotFound)
        {
            _notificationContext.AddNotification("User", "User not found");
            return default!;
        }

        var verifyPassword = BCrypt.Net.BCrypt.Verify(requestDto.Password, user!.Password);

        if (!verifyPassword)
        {
            _notificationContext.AddNotification("Password", "Invalid user or password");
            return default!;
        }

        return user;
    }

    public async Task RegisterAsync(UserRequestDto requestDto)
    {
        requestDto.Validate();
        if (requestDto.Invalid)
        {
            _notificationContext.AddNotifications(requestDto.Notifications);
            return;
        }

        var user = await _userReadRepository.GetByEmailAsync(requestDto.Email);
        if (user != null)
        {
            _notificationContext.AddNotification("Email", "Email already exists");
            return;
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.Password);
        var createUser = Domain.Entities.User.Create(requestDto.Name, requestDto.Email, hashPassword);

        await _writeRepository.InsertAsync(createUser);
    }
}