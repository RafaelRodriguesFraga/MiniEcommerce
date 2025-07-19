using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Request;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Application.Base;
using DotnetBaseKit.Components.Shared.Notifications;

namespace AuthService.Application.Services;

public class UserRegistrationService : BaseServiceApplication, IUserRegistrationService
{
    private readonly IUserWriteRepository _userWriteRepository;

    public UserRegistrationService(NotificationContext notificationContext, IUserWriteRepository userWriteRepository)
        : base(notificationContext)
    {
        _userWriteRepository = userWriteRepository;
    }

    public async Task RegisterUserAsync(RegisterUserRequestDto requestDto)
    {
        requestDto.Validate();
        if (requestDto.Invalid)
        {
            _notificationContext.AddNotifications(requestDto.Notifications);
            return;
        }
        
        var emailExists = await _userWriteRepository.GetByEmailAsync(requestDto.Email);
        if (emailExists)
        {
            _notificationContext.AddNotification("Email", "Email already exists");
            return;
        }

        var hashPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.Password);
        var user = User.Create(requestDto.Name, requestDto.Email, hashPassword);

        await _userWriteRepository.InsertAsync(user);
    }
}