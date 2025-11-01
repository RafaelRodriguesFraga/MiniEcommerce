using AuthService.Application.DTOs.Login;
using AuthService.Application.DTOs.User;
using AuthService.Application.Services.User;
using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using DotnetBaseKit.Components.Shared.Notifications;
using Moq;

namespace AuthService.Test.UnitTests.Application.Services;

public class UserServiceApplicationTests
{
    private readonly Mock<IUserReadRepository> _readRepositoryMock;
    private readonly Mock<IUserWriteRepository> _writeRepositoryMock;
    private readonly NotificationContext _notificationContext;
    private readonly UserServiceApplication _service;

    public UserServiceApplicationTests()
    {
        _readRepositoryMock = new Mock<IUserReadRepository>();
        _writeRepositoryMock = new Mock<IUserWriteRepository>();
        _notificationContext = new NotificationContext();
        _service = new UserServiceApplication(_notificationContext, _readRepositoryMock.Object, _writeRepositoryMock.Object);
    }

    [Fact(DisplayName = "Should return notification when user not found")]
    public async Task AuthenticateAsync_UserNotFound_ShouldReturnNotification()
    {
        _readRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var login = new LoginRequestDto { Email = "test@example.com", Password = "123456" };

        var result = await _service.AuthenticateAsync(login);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "User");
    }

    [Fact(DisplayName = "Should return notification when password is invalid")]
    public async Task AuthenticateAsync_InvalidPassword_ShouldReturnNotification()
    {
        var user = User.Create("Test", "test@example.com", BCrypt.Net.BCrypt.HashPassword("correct"));
        _readRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var login = new LoginRequestDto { Email = user.Email, Password = "wrong" };

        var result = await _service.AuthenticateAsync(login);

        Assert.Null(result);
        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Password");
    }

    [Fact(DisplayName = "Should return user when credentials are correct")]
    public async Task AuthenticateAsync_ValidCredentials_ShouldReturnUser()
    {
        var password = "123456";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = User.Create("Test", "test@example.com", hashedPassword);

        _readRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        var login = new LoginRequestDto { Email = user.Email, Password = password };

        var result = await _service.AuthenticateAsync(login);

        Assert.NotNull(result);
        Assert.False(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should return notification when registering user with invalid data")]
    public async Task RegisterAsync_InvalidDto_ShouldReturnNotification()
    {
        var dto = new UserRequestDto(); // inválido pois não foi preenchido
        dto.Validate();

        await _service.RegisterAsync(dto);

        Assert.True(_notificationContext.HasNotifications);
    }

    [Fact(DisplayName = "Should return notification when email already exists")]
    public async Task RegisterAsync_EmailAlreadyExists_ShouldReturnNotification()
    {
        var dto = new UserRequestDto
        {
            Name = "Test",
            Email = "test@example.com",
            Password = "123456"
        };
        dto.Validate();

        _readRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync(User.Create(dto.Name, dto.Email, "hash"));

        await _service.RegisterAsync(dto);

        Assert.True(_notificationContext.HasNotifications);
        Assert.Contains(_notificationContext.Notifications, n => n.Key == "Email");
    }

    [Fact(DisplayName = "Should register user successfully")]
    public async Task RegisterAsync_ValidDto_ShouldRegisterUser()
    {
        var dto = new UserRequestDto
        {
            Name = "Test",
            Email = "test@example.com",
            Password = "123456"
        };
        dto.Validate();

        _readRepositoryMock.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        await _service.RegisterAsync(dto);

        _writeRepositoryMock.Verify(w => w.InsertAsync(It.IsAny<User>()), Times.Once);
        Assert.False(_notificationContext.HasNotifications);
    }
}